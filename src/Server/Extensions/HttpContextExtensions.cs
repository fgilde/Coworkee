using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Application.Security;
using CleanArchitectureBase.Shared.Constants.Application;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;


namespace CleanArchitectureBase.Server.Extensions
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

            if (string.IsNullOrWhiteSpace(userId))
            {
                context.RedirectToClient("/?ReturnUrl=" + context.Request.GetEncodedUrl());
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