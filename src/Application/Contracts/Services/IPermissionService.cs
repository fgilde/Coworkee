using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Security;

namespace CleanArchitectureBase.Application.Contracts.Services
{
    public interface IPermissionService
    {
        /// <summary>
        /// Ensures specific policy
        /// </summary>
        /// <param name="policy">Policy to check</param>
        /// <param name="userId">User to ensure policy for or null for current</param>
        /// <returns></returns>
        Task EnsurePolicyAsync(string policy, string userId = null);

        /// <summary>
        /// Ensures specific policies
        /// </summary>
        /// <param name="policies">Policies to check</param>
        /// <param name="match">Set Match.All to ensure all policies are granted or any</param>
        /// <param name="userId">User to ensure policy for or null for current</param>
        /// <returns></returns>
        Task EnsurePoliciesAsync(string[] policies, PolicyMatch match, string userId = null);

        /// <summary>
        /// Returns true if given or current user hat given policy
        /// </summary>
        /// <param name="policy">Policy to check</param>
        /// <param name="userId">User to ensure policy for or null for current</param>
        /// <returns></returns>
        Task<bool> HasPolicyAsync(string policy, string userId = null);

        /// <summary>
        /// Returns true if given or current user hat given policy
        /// </summary>
        /// <param name="policies">Policies to check</param>
        /// <param name="match">Set Match.All to ensure all policies are granted or any</param>
        /// <param name="userId">User to ensure policy for or null for current</param>
        /// <returns></returns>
        Task<bool> HasPoliciesAsync(string[] policies, PolicyMatch match, string userId = null);

        /// <summary>
        /// Ensures specific role
        /// </summary>
        /// <param name="role">Role to check</param>
        /// <param name="userId">User to ensure policy for or null for current</param>
        /// <returns></returns>
        Task EnsureRoleAsync(string role, string userId = null);

        /// <summary>
        /// Ensures specific roles
        /// </summary>
        /// <param name="roles">Role to check</param>
        /// <param name="match">Set Match.All to ensure all policies are granted or any</param>
        /// <param name="userId">User to ensure policy for or null for current</param>
        /// <returns></returns>
        Task EnsureRolesAsync(string[] roles, RoleMatch match, string userId = null);

        /// <summary>
        /// Returns true if given or current user is in given role
        /// </summary>
        /// <param name="role">Role to check</param>
        /// <param name="userId">User to ensure policy for or null for current</param>
        /// <returns></returns>
        Task<bool> HasRoleAsync(string role, string userId = null);

        /// <summary>
        /// Returns true if given or current user is in one or all given roles
        /// </summary>
        /// <param name="roles">Roles to check</param>
        /// <param name="match">Set Match.All to ensure all policies are granted or any</param>
        /// <param name="userId">User to ensure policy for or null for current</param>
        /// <returns></returns>
        Task<bool> HasRolesAsync(string[] roles, RoleMatch match, string userId = null);

        /// <summary>
        /// Returns true if given or current user is an Administrator
        /// </summary>
        /// <param name="userId">User to ensure policy for or null for current</param>
        /// <returns></returns>
        Task<bool> IsAdministratorAsync(string userId = null);
    }
}