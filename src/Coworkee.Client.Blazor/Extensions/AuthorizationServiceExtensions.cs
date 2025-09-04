using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coworkee.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;

namespace Coworkee.Client.Extensions
{
    public static class AuthorizationServiceExtensions
    {
        public static async Task<bool> HasPoliciesAsync(this IAuthorizationService authorizationService, ClaimsPrincipal user, PolicyMatch match, params string[] policies)
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