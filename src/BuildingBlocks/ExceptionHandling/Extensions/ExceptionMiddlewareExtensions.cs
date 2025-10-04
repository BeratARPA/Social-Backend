using ExceptionHandling.Middleware;
using Microsoft.AspNetCore.Builder;

namespace ExceptionHandling.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
       
            return app;
        }
    }
}
