using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AspNetCoreEx.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly int _limit;
        private readonly TimeSpan _timeWindow;
        private static readonly ConcurrentDictionary<string, (DateTime, int)> _requests = new();

        public RateLimitingMiddleware(RequestDelegate next, int limit, TimeSpan timeWindow)
        {
            _next = next;
            _limit = limit;
            _timeWindow = timeWindow;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var key = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            var currentTime = DateTime.UtcNow;

            var (lastRequestTime, requestCount) = _requests.GetOrAdd(key, (currentTime, 0));

            if (currentTime - lastRequestTime < _timeWindow)
            {
                if (requestCount >= _limit)
                {
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsync("Too many requests. Please try again later.");
                    return;
                }
                _requests[key] = (lastRequestTime, requestCount + 1);
            }
            else
            {
                _requests[key] = (currentTime, 1);
            }

            await _next(context);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class RateLimitingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRateLimitingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RateLimitingMiddleware>();
        }
    }
}
