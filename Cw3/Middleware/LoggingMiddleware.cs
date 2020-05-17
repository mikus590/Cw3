using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cw3.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.EnableBuffering();
            if (httpContext.Request != null)
            {
                string path = httpContext.Request.Path;
                string method = httpContext.Request.Method;
                string queryString = httpContext.Request.QueryString.ToString();

                string bodyStr = "";

                using (var reader = new StreamReader(
                    httpContext.Request.Body,
                    Encoding.UTF8,
                    true,
                    1024,
                    true))
                {
                    bodyStr = await reader.ReadToEndAsync();
                    httpContext.Request.Body.Position = 0;
                }
                string logPath = @"D:\requestsLog.txt";
                if (path == null) path = "null";
                if (method == null) method = "null";
                if (queryString == null) queryString = "null";
                string toLog = DateTime.Now + " path: " + path + " query String: " + queryString + " method: " + method + " body: " + bodyStr;
                File.WriteAllText(logPath, toLog);
            }

            if(_next!=null) await _next(httpContext);
        }
    }
}
