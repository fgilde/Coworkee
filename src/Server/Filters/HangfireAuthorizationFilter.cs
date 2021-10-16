using CleanArchitectureBase.Server.Extensions;
using CleanArchitectureBase.Shared.Constants.Permission;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace CleanArchitectureBase.Server.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            if (httpContext.RedirectToLoginIfUnauthorized())
                return true; // Its very strange and we are not logging in, but otherwise redirect will aborted
            return !httpContext.SetStatusIfPolicyMissingAsync(StatusCodes.Status403Forbidden, Permissions.Hangfire.View).Result;
        }
    }
}