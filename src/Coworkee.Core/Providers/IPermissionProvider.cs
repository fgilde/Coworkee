using System.Collections.Generic;

namespace Coworkee.Core.Providers
{
    /// <summary>
    /// Interface for providing permission constants in a modular way.
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// Gets all permissions provided by this provider.
        /// </summary>
        /// <returns>Collection of permission strings.</returns>
        IEnumerable<string> GetPermissions();

        /// <summary>
        /// Gets required dependency permissions for a specific permission.
        /// </summary>
        /// <param name="permission">The permission to check dependencies for.</param>
        /// <returns>Array of required dependency permissions.</returns>
        string[] GetRequiredDependencyPermissions(string permission);
    }

    /// <summary>
    /// Base implementation of permission provider.
    /// </summary>
    public abstract class PermissionProviderBase : IPermissionProvider
    {
        public abstract IEnumerable<string> GetPermissions();
        public abstract string[] GetRequiredDependencyPermissions(string permission);
    }
}