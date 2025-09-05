using System.Collections.Generic;

namespace lib.Coworkee.Core.Providers
{
    /// <summary>
    /// Interface for providing role constants in a modular way.
    /// </summary>
    public interface IRoleProvider
    {
        /// <summary>
        /// Gets all roles provided by this provider.
        /// </summary>
        /// <returns>Collection of role names.</returns>
        IEnumerable<string> GetRoles();

        /// <summary>
        /// Gets the description for a specific role.
        /// </summary>
        /// <param name="roleName">The role name.</param>
        /// <returns>Role description or null if not found.</returns>
        string GetRoleDescription(string roleName);
    }

    /// <summary>
    /// Base implementation of role provider.
    /// </summary>
    public abstract class RoleProviderBase : IRoleProvider
    {
        public abstract IEnumerable<string> GetRoles();
        public abstract string GetRoleDescription(string roleName);
    }
}