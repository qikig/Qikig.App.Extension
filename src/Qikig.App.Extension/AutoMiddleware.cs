using Microsoft.AspNetCore.Http;

namespace Qikig.App.Extension
{
    /// <summary>
    /// 统一中间件注册
    /// </summary>
    public class AutoMiddleware
    {
        private readonly RequestDelegate _next;
        public AutoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
        }
    }
}
