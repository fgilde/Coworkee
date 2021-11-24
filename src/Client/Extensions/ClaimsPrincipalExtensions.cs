using System;
using System.Linq;
using System.Security.Claims;
using CleanArchitectureBase.Application.Common.Security;

namespace CleanArchitectureBase.Client.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        public static string GetInitials(this ClaimsPrincipal claimsPrincipal)
            => new(new []{claimsPrincipal.GetFirstName().FirstOrDefault(), claimsPrincipal.GetLastName().FirstOrDefault()});

        public static string GetFullName(this ClaimsPrincipal claimsPrincipal)
            => $"{claimsPrincipal.GetFirstName()} {claimsPrincipal.GetLastName()}";

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Email);

        public static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Name);

        public static string GetLastName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Surname);

        public static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);

        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
           => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);

        public static bool IsGuest(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) == Guid.Empty.ToString();
        
        public static bool HasRoles(this ClaimsPrincipal user, RoleMatch match, string[] roles)
        {
            var granted = true;
            foreach (var role in roles ?? Enumerable.Empty<string>().ToArray())
            {
                granted = user.IsInRole(role);
                if (!granted && match == RoleMatch.All)
                    return false;
                if (granted && match == RoleMatch.Any)
                    return true;
            }
            return granted;
        }
    }
}