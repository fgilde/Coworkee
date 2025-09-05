using lib.Coworkee.Shared.Constants.Role;
using System.Collections.Generic;
using lib.Coworkee.Core.Providers;

namespace lib.Coworkee.Shared.Constants.Role
{
    /// <summary>
    /// Core role constants for base system roles.
    /// </summary>
    public static class CoreRoleConstants
    {
        public const string AdministratorRole = "Administrator";
        public const string BasicRole = "Basic";
    }

    /// <summary>
    /// Core role provider for base system roles.
    /// Business-specific roles should be provided by application-level role providers.
    /// </summary>
    public class CoreRoleProvider : RoleProviderBase
    {
        private readonly Dictionary<string, string> _roleDescriptions = new()
        {
            { CoreRoleConstants.AdministratorRole, "System administrator with full access" },
            { CoreRoleConstants.BasicRole, "Basic user with limited access" }
        };

        /// <inheritdoc />
        public override IEnumerable<string> GetRoles()
        {
            yield return CoreRoleConstants.AdministratorRole;
            yield return CoreRoleConstants.BasicRole;
        }

        /// <inheritdoc />
        public override string GetRoleDescription(string roleName)
        {
            return _roleDescriptions.TryGetValue(roleName, out var description) ? description : null;
        }
    }
}