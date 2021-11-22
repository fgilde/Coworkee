using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Responses.Audit;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Constants.Role;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Server.Controllers.Utilities
{
    [ApiController]
    [Authorize]
    public class AuditsController : BaseApiController<AuditsController>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IAuditService _auditService;

        public AuditsController(ICurrentUserService currentUserService, IAuditService auditService)
        {
            _currentUserService = currentUserService;
            _auditService = auditService;
        }

        /// <summary>
        /// Get Current User Audit Trails
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.AuditTrails.View)]
        [HttpGet]
        [Produces(typeof(Result<IEnumerable<AuditResponse>>))]
        public async Task<IActionResult> GetUserTrailsAsync([FromQuery] string[] userIds, CancellationToken cancellationToken = default)
        {
            userIds ??= new[] {_currentUserService.UserId};
            if (userIds.Length != 1 || userIds[0] != _currentUserService.UserId) // if a user filter is applied that is different than current user, only allowed by admins
                await Get<IPermissionService>().EnsureRoleAsync(RoleConstants.AdministratorRole);
            return Ok(await _auditService.GetTrailsAsync(1000, userIds));
        }

        /// <summary>
        /// Search Audit Trails and Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="searchInOldValues"></param>
        /// <param name="searchInNewValues"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.AuditTrails.Export)]
        [HttpGet("export")]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> ExportExcel([FromQuery] string[] userIds, string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false)
        {
            userIds ??= new[] { _currentUserService.UserId };
            if (userIds.Length != 1 || userIds[0] != _currentUserService.UserId) // if a user filter is applied that is different than current user, only allowed by admins
                await Get<IPermissionService>().EnsureRoleAsync(RoleConstants.AdministratorRole);
            var data = await _auditService.ExportAsync(userIds, searchString, searchInOldValues, searchInNewValues);
            return Ok(data);
        }
    }
}