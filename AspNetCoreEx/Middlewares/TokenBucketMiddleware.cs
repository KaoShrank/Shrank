namespace AspNetCoreEx.Middleware
{
    using AspNetCoreEx.Models;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Token桶演算法
    /// </summary>
    public class TokenBucketMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly TokenBucket _tokenBucket;

        public TokenBucketMiddleware(RequestDelegate next, TokenBucket tokenBucket)
        {
            _next = next;
            _tokenBucket = tokenBucket;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (_tokenBucket.TryConsume(1))
            {
                // Token充足，繼續處理請求
                await _next(context);
            }
            else
            {
                // Token不足，返回429狀態碼
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Too many requests. Please try again later.");
            }
        }
    }
}
