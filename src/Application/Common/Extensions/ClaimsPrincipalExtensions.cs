using System;
using System.Linq;
using System.Security.Claims;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Constants.Role;

namespace CleanArchitectureBase.Application.Common.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool IsAdministrator(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.IsInRole(RoleConstants.AdministratorRole);
        
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

        public static bool HasPolicy(this ClaimsPrincipal user, string policy)
            => user.HasPolicies(PolicyMatch.All, policy);
        public static bool HasPolicies(this ClaimsPrincipal user, string[] policies, PolicyMatch match = PolicyMatch.All)
            => user.HasPolicies(match, policies);
        public static bool HasPolicies(this ClaimsPrincipal user, PolicyMatch match, params string[] policies)
        {
            var granted = true;
            foreach (var permission in policies ?? Enumerable.Empty<string>().ToArray())
            {
                granted = user.HasClaim(c => c.Type == ApplicationClaimTypes.Permission && c.Value == permission);
                if (!granted && match == PolicyMatch.All)
                    return false;
                if (granted && match == PolicyMatch.Any)
                    return true;
            }
            return granted;
        }

        public static bool HasRole(this ClaimsPrincipal user, string role)
            => user.IsInRole(role);
        public static bool HasRoles(this ClaimsPrincipal user, string[] roles, RoleMatch match = RoleMatch.All)
            => user.HasRoles(match, roles);
        public static bool HasRoles(this ClaimsPrincipal user, RoleMatch match, params string[] roles)
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