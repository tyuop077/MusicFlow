using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MusicFlow.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public void AddHeader(HttpContext context, string key, string value)
        {
            if (context.Response.Headers.ContainsKey(key))
            {
                context.Response.Headers.Remove(key);
            }
            context.Response.Headers.Add(key, value);
        }

        public Task Invoke(HttpContext context)
        {
            string token = context.Request.Cookies["token"];
            if (!string.IsNullOrEmpty(token))
                AddHeader(context, "Authorization", "Bearer " + token);

            AddHeader(context, "X-Content-Type-Options", "nosniff");
            AddHeader(context, "X-Xss-Protection", "1");
            AddHeader(context, "X-Frame-Options", "DENY");

            return _next(context);
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }
}
