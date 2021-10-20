using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Security;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitectureBase.Client.Extensions
{
    public static class AuthorizationServiceExtensions
    {
        public static async Task<bool> HasPoliciesAsync(this IAuthorizationService authorizationService, ClaimsPrincipal user, PolicyMatch match, string[] policies)
        {
            var granted = true;
            foreach (var policy in policies ?? Enumerable.Empty<string>().ToArray())
            {
                granted = (await authorizationService.AuthorizeAsync(user, policy)).Succeeded;
                if (!granted && match == PolicyMatch.All)
                    return false;
                if (granted && match == PolicyMatch.Any)
                    return true;
            }
            return granted;
        }
    }
}