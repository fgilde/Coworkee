using CleanArchitectureBase.Application.Interfaces.Services.Account;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Constants.Permission;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureBase.Server.Filters
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {

        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var service = httpContext.RequestServices.GetService<IAccountService>();
            
            var userId = httpContext.Session.GetString(ApplicationConstants.Hangfire.SessionUserIdKey);
            
            return !string.IsNullOrEmpty(userId) && service != null && service.AuthorizeAsync(userId, Permissions.Hangfire.View).Result;
        }
    }
}