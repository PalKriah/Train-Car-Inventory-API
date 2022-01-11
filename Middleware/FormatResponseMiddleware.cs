using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace Train_Car_Inventory_App.Middleware
{
    public class FormatResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public FormatResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments(new PathString("/api")))
            {
                await _next(context);
                return;
            }

            var guid = Guid.NewGuid();
            context.Request.Headers.Add("guid", guid.ToString());

            var originalBodyStream = context.Response.Body;
            context.Response.Body = new MemoryStream();

            await _next(context);
            
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseBodyText = await new StreamReader(context.Response.Body).ReadToEndAsync();

            var resultJson = new JObject
            {
                {"statusCode", context.Response.StatusCode},
                {"content", responseBodyText == "" ? "" : JToken.Parse(responseBodyText)},
                {"identity", guid}
            };

            var resultByteArray = Encoding.UTF8.GetBytes(resultJson.ToString());

            context.Response.ContentLength = resultByteArray.Length;
            await originalBodyStream.WriteAsync(resultByteArray);
        }
    }
}