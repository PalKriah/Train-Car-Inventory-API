using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Train_Car_Inventory_App.Middleware
{
    public class FilterTrainCarsForRailwayWorkersMiddleware
    {
        private readonly RequestDelegate _next;

        public FilterTrainCarsForRailwayWorkersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments(new PathString("/api/traincar")) ||
                context.Request.Path.StartsWithSegments(new PathString("/api/traincar/getstatistics")) ||
                !context.User.HasClaim("IsRailwayWorker", "True"))
            {
                await _next(context);
                return;
            }

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var jToken = JToken.Parse(responseBodyText);
            var railwayCompany = context.User.FindFirst(c => c.Type == "RailwayCompany").Value;

            var filteredResult = FilterResult(jToken, railwayCompany);

            var byteArray = Encoding.UTF8.GetBytes(filteredResult.ToString());
            var stream = new MemoryStream(byteArray);

            context.Response.Body = stream;
        }

        private static JToken FilterResult(JToken json, string railwayCompany)
        {
            if (json.GetType() == typeof(JArray))
            {
                var trains = (JArray) json;
                for (var i = 0; i < trains.Count; i++)
                {
                    if (trains[i]["owner"].ToString() == railwayCompany) continue;
                    trains.RemoveAt(i);
                    i--;
                }

                return json;
            }

            if (json.GetType() == typeof(JObject) && json["owner"].ToString() == railwayCompany)
            {
                return json;
            }

            return new JObject();
        }
    }
}