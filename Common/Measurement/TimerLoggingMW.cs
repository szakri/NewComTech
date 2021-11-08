using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Common.Logs
{
    public class TimerLoggingMW
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private Stopwatch stopWatch = new Stopwatch();
        private static string currentDirectory = Directory.GetCurrentDirectory();
        private static string file = currentDirectory + "\\Measurement\\Log.txt";

        public TimerLoggingMW(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<TimerLoggingMW>();
        }

        public async Task Invoke(HttpContext context)
        {
            string body = "";
            if (context.Request?.Method == "POST")
            {
                context.Request.EnableBuffering();
                var reader = new StreamReader(context.Request?.Body, Encoding.UTF8);
                body = await reader.ReadToEndAsync();
                body = body.Replace("\\n", "").Replace("  ", " ");
                context.Request?.Body.Seek(0, SeekOrigin.Begin);
            }

            using (var buffer = new MemoryStream())
            {
                var response = context.Response;
                var bodyStream = response.Body;
                response.Body = buffer;

                stopWatch.Restart();
                await _next(context);
                stopWatch.Stop();

                buffer.Position = 0;
                await buffer.CopyToAsync(bodyStream);

                bool toBeLogged = (bool)(!context.Request?.Path.Value.Contains("json")) &&
                (bool)(!context.Request?.Path.Value.Contains("html")) &&
                (bool)(!context.Request?.Path.Value.Contains("css")) &&
                (bool)(!context.Request?.Path.Value.Contains("favicon")) &&
                (bool)(!context.Request?.Path.Value.Contains("js")) &&
                (bool)(!context.Request?.Path.Value.Contains("ttf"));
                if (toBeLogged)
                {
                    body = string.IsNullOrEmpty(body) ? "" : " body: " + body;
                    string text = $"{DateTime.Now}\t{stopWatch.ElapsedMilliseconds} ms\t{context.Response?.ContentLength ?? buffer.Length} B\t" +
                        $"{context.Request?.Path.Value}{context.Request?.QueryString}{body}";
                    _logger.LogInformation(text);
                    File.AppendAllText(file, text + "\n");
                }
            }
        }
    }
}
