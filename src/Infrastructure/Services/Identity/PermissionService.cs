using System;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Exceptions;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Attributes;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Contracts.Services.Account;

namespace CleanArchitectureBase.Infrastructure.Services.Identity
{
    [RegisterAs(typeof(IPermissionService), 4)]
    public class PermissionService: IPermissionService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAccountService _accountService;

        public PermissionService(
            ICurrentUserService currentUserService,
            IAccountService accountService)
        {
            _currentUserService = currentUserService;
            _accountService = accountService;
        }

        public async Task<bool> HasRoleAsync(string role, string userId = null)
        {
            userId = UserId(userId);
            if (userId == null)
                return false;
            return await _accountService.IsInRoleAsync(userId, role);
        }

        public async Task<bool> HasPolicyAsync(string policy, string userId = null)
        {
            userId = UserId(userId);
            if (userId == null)
                return false;
            return await _accountService.AuthorizeAsync(userId, policy);
        }

        public async Task<bool> HasRolesAsync(string[] roles, RoleMatch match, string userId = null)
        {
            var granted = true;
            foreach (var role in roles ?? Enumerable.Empty<string>().ToArray())
            {
                granted = await HasRoleAsync(role, userId);
                if (!granted && match == RoleMatch.All)
                    return false;
                if (granted && match == RoleMatch.Any)
                    return true;
            }
            return granted;
        }

        public async Task<bool> HasPoliciesAsync(string[] policies, PolicyMatch match, string userId = null)
        {
            var granted = true;
            foreach (var policy in policies ?? Enumerable.Empty<string>().ToArray())
            {
                granted = await HasPolicyAsync(policy, userId);
                if (!granted && match == PolicyMatch.All)
                    return false;
                if (granted && match == PolicyMatch.Any)
                    return true;
            }
            return granted;
        }

        public async Task EnsurePolicyAsync(string policy, string userId = null)
        {
            userId = UserId(userId);
            if (!string.IsNullOrEmpty(policy) && !await HasPolicyAsync(policy))
                throw CreateException(userId);
        }

        public async Task EnsureRoleAsync(string role, string userId = null)
        {
            userId = UserId(userId);
            if (!string.IsNullOrEmpty(role) && !await HasRoleAsync(role))
                throw CreateException(userId);
        }

        public async Task EnsurePoliciesAsync(string[] policies, PolicyMatch match, string userId = null)
        {
            userId = UserId(userId);
            if (!await HasPoliciesAsync(policies, match, userId))
                throw CreateException(userId);
        }
        

        public async Task EnsureRolesAsync(string[] roles, RoleMatch match, string userId = null)
        {
            userId = UserId(userId);
            if (!await HasRolesAsync(roles, match, userId))
                throw CreateException(userId);
        }

        private Exception CreateException(string checkedUserId)
        {
            return checkedUserId == null
                ? new UnauthorizedAccessException()
                : new ForbiddenAccessException();
        }
        
        private string UserId(string userId = null)
        {
            return userId ?? _currentUserService.UserId;
        }
    }
}