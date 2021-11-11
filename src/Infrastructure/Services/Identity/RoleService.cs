using CleanArchitectureBase.Application.Interfaces.Services.Identity;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Application.Responses.Identity;
using CleanArchitectureBase.Infrastructure.Helpers;
using CleanArchitectureBase.Infrastructure.Models.Identity;
using CleanArchitectureBase.Shared.Constants.Role;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Shared.Constants.Permission;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Infrastructure.Services.Identity
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleClaimService _roleClaimService;
        private readonly IStringLocalizer<RoleService> _localizer;
        private readonly ICurrentUserService _currentUserService;
        
        public RoleService(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            IRoleClaimService roleClaimService,
            IStringLocalizer<RoleService> localizer,
            ICurrentUserService currentUserService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _roleClaimService = roleClaimService;
            _localizer = localizer;
            _currentUserService = currentUserService;
        }

        public async Task<Result<string>> DeleteAsync(string id)
        {
            var existingRole = await _roleManager.FindByIdAsync(id);
            if (existingRole.Name != RoleConstants.AdministratorRole && existingRole.Name != RoleConstants.BasicRole)
            {
                bool roleIsNotUsed = true;
                var allUsers = await _userManager.Users.ToListAsync();
                foreach (var user in allUsers)
                {
                    if (await _userManager.IsInRoleAsync(user, existingRole.Name))
                    {
                        roleIsNotUsed = false;
                    }
                }
                if (roleIsNotUsed)
                {
                    await _roleManager.DeleteAsync(existingRole);
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Deleted."], existingRole.Name));
                }
                else
                {
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Not allowed to delete {0} Role as it is being used."], existingRole.Name));
                }
            }
            else
            {
                return await Result<string>.SuccessAsync(string.Format(_localizer["Not allowed to delete {0} Role."], existingRole.Name));
            }
        }

        public async Task<Result<List<RoleResponse>>> GetAllAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var rolesResponse = roles.MapTo<List<RoleResponse>>();
            return await Result<List<RoleResponse>>.SuccessAsync(rolesResponse);
        }

        public async Task<Result<PermissionResponse>> GetAllPermissionsAsync(string roleId)
        {
            var model = new PermissionResponse();
            var allPermissions = GetAllPermissions();
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                model.RoleId = role.Id;
                model.RoleName = role.Name;
                var roleClaimsResult = await _roleClaimService.GetAllByRoleIdAsync(role.Id);
                if (roleClaimsResult.Succeeded)
                {
                    var roleClaims = roleClaimsResult.Data;
                    var allClaimValues = allPermissions.Select(a => a.ClaimValue).ToList();
                    var roleClaimValues = roleClaims.Select(a => a.ClaimValue).ToList();
                    var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
                    foreach (var permission in allPermissions)
                    {
                        if (authorizedClaims.Any(a => a == permission.ClaimValue))
                        {
                            permission.Selected = true;
                            var roleClaim = roleClaims.SingleOrDefault(a => a.ClaimValue == permission.ClaimValue);
                            if (roleClaim?.Description != null)
                            {
                                permission.Description = roleClaim.Description;
                            }
                            if (roleClaim?.Group != null)
                            {
                                permission.Group = roleClaim.Group;
                            }
                        }
                    }
                }
                else
                {
                    model.RoleClaims = new List<RoleClaimResponse>();
                    return await Result<PermissionResponse>.FailAsync(roleClaimsResult.Messages);
                }
            }
            model.RoleClaims = allPermissions;
            return await Result<PermissionResponse>.SuccessAsync(model);
        }

        private List<RoleClaimResponse> GetAllPermissions()
        {
            var allPermissions = new List<RoleClaimResponse>();

            #region GetPermissions

            allPermissions.GetAllPermissions();

            #endregion GetPermissions

            return allPermissions;
        }

        public async Task<Result<RoleResponse>> GetByIdAsync(string id)
        {
            var roles = await _roleManager.Roles.SingleOrDefaultAsync(x => x.Id == id);
            var rolesResponse = roles.MapTo<RoleResponse>();
            return await Result<RoleResponse>.SuccessAsync(rolesResponse);
        }

        public async Task<Result<string>> SaveAsync(RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.Id))
            {
                var existingRole = await _roleManager.FindByNameAsync(request.Name);
                if (existingRole != null) return await Result<string>.FailAsync(_localizer["Similar Role already exists."]);
                var response = await _roleManager.CreateAsync(new ApplicationRole(request.Name, request.Description));
                if (response.Succeeded)
                {
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Created."], request.Name));
                }
                else
                {
                    return await Result<string>.FailAsync(response.Errors.Select(e => _localizer[e.Description].ToString()).ToList());
                }
            }
            else
            {
                var existingRole = await _roleManager.FindByIdAsync(request.Id);
                if (existingRole.Name == RoleConstants.AdministratorRole || existingRole.Name == RoleConstants.BasicRole)
                {
                    return await Result<string>.FailAsync(string.Format(_localizer["Not allowed to modify {0} Role."], existingRole.Name));
                }
                existingRole.Name = request.Name;
                existingRole.NormalizedName = request.Name.ToUpper();
                existingRole.Description = request.Description;
                await _roleManager.UpdateAsync(existingRole);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Updated."], existingRole.Name));
            }
        }

        public async Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request)
        {
            try
            {
                var errors = new List<string>();
                var role = await _roleManager.FindByIdAsync(request.RoleId);
                if (role.Name == RoleConstants.AdministratorRole)
                {
                    var currentUser = await _userManager.Users.SingleAsync(x => x.Id == _currentUserService.UserId);
                    if (await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
                    {
                        return await Result<string>.FailAsync(_localizer["Not allowed to modify Permissions for this Role."]);
                    }
                }

                var selectedClaims = request.RoleClaims.Where(a => a.Selected).ToList();
                if (role.Name == RoleConstants.AdministratorRole)
                {
                    if (!selectedClaims.Any(x => x.ClaimValue == Permissions.Roles.View)
                       || !selectedClaims.Any(x => x.ClaimValue == Permissions.RoleClaims.View)
                       || !selectedClaims.Any(x => x.ClaimValue == Permissions.RoleClaims.Edit))
                    {
                        return await Result<string>.FailAsync(string.Format(
                            _localizer["Not allowed to deselect {0} or {1} or {2} for this Role."],
                            Permissions.Roles.View, Permissions.RoleClaims.View, Permissions.RoleClaims.Edit));
                    }
                }

                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }
                foreach (var claim in selectedClaims)
                {
                    var addResult = await _roleManager.AddPermissionClaim(role, claim.ClaimValue);
                    if (!addResult.Succeeded)
                    {
                        errors.AddRange(addResult.Errors.Select(e => _localizer[e.Description].ToString()));
                    }
                }

                var addedClaims = await _roleClaimService.GetAllByRoleIdAsync(role.Id);
                if (addedClaims.Succeeded)
                {
                    foreach (var claim in selectedClaims)
                    {
                        var addedClaim = addedClaims.Data.SingleOrDefault(x => x.ClaimType == claim.ClaimType && x.ClaimValue == claim.ClaimValue);
                        if (addedClaim != null)
                        {
                            claim.Id = addedClaim.Id;
                            claim.RoleId = addedClaim.RoleId;
                            var saveResult = await _roleClaimService.SaveAsync(claim);
                            if (!saveResult.Succeeded)
                            {
                                errors.AddRange(saveResult.Messages);
                            }
                        }
                    }
                }
                else
                {
                    errors.AddRange(addedClaims.Messages);
                }

                if (errors.Any())
                {
                    return await Result<string>.FailAsync(errors);
                }

                return await Result<string>.SuccessAsync(_localizer["Permissions Updated."]);
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync(ex.Message);
            }
        }

        public async Task<int> GetCountAsync()
        {
            var count = await _roleManager.Roles.CountAsync();
            return count;
        }
    }
}