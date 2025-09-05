using System.Security.Claims;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Configurations;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Nextended.Core.Extensions;

namespace Coworkee.Server.Extensions
{
    public static class HttpContextExtensions
    {
        public static string GetUserId(this HttpContext context)
        {
            return context?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public static HttpContext RedirectToClient(this HttpContext context, string url = "/")
        {
            context.Response.Redirect(url);
            return context;
        }

        public static bool RedirectToLoginIfUnauthorized(this HttpContext context)
        {
            var userId = context.GetUserId() ?? context.Session.GetString(ApplicationConstants.Session.SessionUserIdKey);
            var config = context.RequestServices.GetService<ServerConfiguration>();

            if (string.IsNullOrWhiteSpace(userId))
            {                
                var baseUrl = !ApplicationConstants.HostClientInServer ? config?.ClientUrl.EnsureEndsWith("/") : "/";
                context.RedirectToClient($"{baseUrl}{ApplicationConstants.Routes.Login}?{ApplicationConstants.ParameterNames.ReturnUrl}=" + context.Request.GetEncodedUrl());
                return true;
            }
            return false;
        }

        public static async Task<bool> SetStatusIfPolicyMissingAsync(this HttpContext context, int statusCode, params string[] policies)
        {
            var userId = context.GetUserId() ?? context.Session.GetString(ApplicationConstants.Session.SessionUserIdKey);
            var hasPolicies = await context.RequestServices.GetService<IPermissionService>().HasPoliciesAsync(policies, PolicyMatch.All, userId);
            return context.SetStatusIf(statusCode, !hasPolicies);
        }

        public static bool SetStatusIf(this HttpContext context, int statusCode, bool condition)
        {
            if (condition)
                context.Response.StatusCode = statusCode;
            return condition;
        }
    }
}