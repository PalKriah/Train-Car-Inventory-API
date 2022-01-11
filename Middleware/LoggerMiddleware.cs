using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json.Linq;

namespace Train_Car_Inventory_App.Middleware
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggerMiddleware(RequestDelegate next)
        {
            if (!File.Exists("log.txt"))
            {
                File.Create("log.txt");
            }

            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments(new PathString("/api")) ||
                !context.User.Identity.IsAuthenticated)
            {
                await _next(context);
                return;
            }

            var requestBodyContent = await GetRequestBodyContent(context.Request);

            var content = string.IsNullOrEmpty(requestBodyContent) ? new JObject() : JObject.Parse(requestBodyContent);

            var logJson = new JObject
            {
                {"method", context.Request.Method},
                {"endpoint", context.Request.GetEncodedPathAndQuery()},
                {"content", content},
                {"id", context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value},
                {"userName", context.User.Identity.Name}
            };
            await WriteToFile(logJson);
            await _next(context);
        }

        private async Task<string> GetRequestBodyContent(HttpRequest request)
        {
            request.EnableBuffering();

            var bodyText = await new StreamReader(request.Body).ReadToEndAsync();

            request.Body.Position = 0;

            return bodyText;
        }

        private async Task WriteToFile(JObject logJson)
        {
            var writer = File.AppendText("log.txt");

            writer.WriteLine(logJson);
            writer.WriteLine();

            await writer.FlushAsync();

            writer.Close();
        }
    }
}