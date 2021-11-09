using System;
using System.Linq;
using System.Security.Claims;

namespace CleanArchitectureBase.Client.Extensions
{
    internal static class ClaimsPrincipalExtensions
    {
        internal static string GetInitials(this ClaimsPrincipal claimsPrincipal)
            => new(new []{claimsPrincipal.GetFirstName().FirstOrDefault(), claimsPrincipal.GetLastName().FirstOrDefault()});

        internal static string GetFullName(this ClaimsPrincipal claimsPrincipal)
            => $"{claimsPrincipal.GetFirstName()} {claimsPrincipal.GetLastName()}";

        internal static string GetEmail(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Email);

        internal static string GetFirstName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Name);

        internal static string GetLastName(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.Surname);

        internal static string GetPhoneNumber(this ClaimsPrincipal claimsPrincipal)
            => claimsPrincipal.FindFirstValue(ClaimTypes.MobilePhone);

        internal static string GetUserId(this ClaimsPrincipal claimsPrincipal)
           => claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}