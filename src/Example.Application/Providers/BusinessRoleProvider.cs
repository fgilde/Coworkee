using System.Collections.Generic;
using Coworkee.Core.Providers;

namespace Example.Application.Providers
{
    /// <summary>
    /// Business-specific role provider for the example application.
    /// Demonstrates how to extend core roles with business roles.
    /// </summary>
    public class BusinessRoleProvider : RoleProviderBase
    {
        private readonly Dictionary<string, string> _roleDescriptions = new()
        {
            { "ProductManager", "Product manager with full access to product management features" },
            { "BrandManager", "Brand manager with full access to brand management features" },
            { "DocumentManager", "Document manager with access to document management features" }
        };

        /// <inheritdoc />
        public override IEnumerable<string> GetRoles()
        {
            yield return "ProductManager";
            yield return "BrandManager";
            yield return "DocumentManager";
        }

        /// <inheritdoc />
        public override string GetRoleDescription(string roleName)
        {
            return _roleDescriptions.TryGetValue(roleName, out var description) ? description : null;
        }
    }
}