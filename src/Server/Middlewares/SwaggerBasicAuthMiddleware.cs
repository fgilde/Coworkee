using System;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Server.Extensions;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureBase.Server.Middlewares
{

    public class SwaggerAuthorizedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceProvider _serviceProvider;

        public SwaggerAuthorizedMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            _next = next;
            _serviceProvider = serviceProvider;
        }

        public async Task Invoke(HttpContext context)
        {
            var configuration = _serviceProvider.GetRequiredService<ServerConfiguration>();
            if (configuration.ApiDocumentation.RequireLogin && context.Request.Path.StartsWithSegments("/swagger") &&
                (context.RedirectToLoginIfUnauthorized() || (configuration.ApiDocumentation.RequirePermission && await context.SetStatusIfPolicyMissingAsync(StatusCodes.Status403Forbidden, Permissions.Swagger.View))))
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