using System.Threading.Tasks;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Server.Extensions;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace CleanArchitectureBase.Server.Middlewares
{

    public class SwaggerAuthorizedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ServerConfiguration _configuration;

        public SwaggerAuthorizedMiddleware(RequestDelegate next, ServerConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            if (_configuration.ApiDocumentation.RequireLogin && context.Request.Path.StartsWithSegments("/swagger") && 
                (context.RedirectToLoginIfUnauthorized() || (_configuration.ApiDocumentation.RequirePermission && await context.SetStatusIfPolicyMissingAsync(StatusCodes.Status403Forbidden, Permissions.Swagger.View))))
            {
                return;
            }

            await _next.Invoke(context);
        }
    }

    public static class SwaggerAuthorizeExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerAuthorizedMiddleware>();
        }
    }
}