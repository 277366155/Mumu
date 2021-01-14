using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebAppTest.Middlewares
{
    public static class MyTestMiddlewareExtensions
    {
        public static void UseMyTestMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<MyTestMiddleware>();
        }
    }

    public class MyTestMiddleware
    {
        RequestDelegate _next;
        ILogger _logger;
        public MyTestMiddleware(RequestDelegate next, ILogger<MyTestMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            using (_logger.BeginScope("traceid:{traceId}", context.TraceIdentifier))
            {
                _logger.LogWarning("logger 记录开始");
                await _next.Invoke(context);
                _logger.LogWarning("logger 记录结束");
            }
        }
    }
}
