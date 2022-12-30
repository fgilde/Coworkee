using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Enums;
using Coworkee.Application.Contracts.Services;
using Coworkee.Server.Filters;
using Coworkee.Shared.Constants.Permission;
using Coworkee.Shared.Constants.Role;
using HeyRed.Mime;

namespace Coworkee.Server.Controllers.Utilities
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
        [Produces(typeof(IReadOnlyCollection<AuditDto>))]
        public async Task<IActionResult> GetUserTrailsAsync([FromQuery] string[] userIds, CancellationToken cancellationToken = default)
        {
            userIds ??= new[] {_currentUserService.UserId};
            if (userIds.Length != 1 || userIds[0] != _currentUserService.UserId) // if a user filter is applied that is different than current user, only allowed by admins
                await Get<IPermissionService>().EnsureRoleAsync(RoleConstants.AdministratorRole);
            return Ok(await _auditService.GetTrailsAsync(1000, userIds));
        }

        /// <summary>
        /// Exports products
        /// </summary>
        /// <returns></returns>
        [Authorize(Policy = Permissions.AuditTrails.Export)]
        [HttpGet(nameof(Export))]
        [AllowSynchronousIo]
        public async Task<IActionResult> Export(ExportServiceType exportServiceType,
            [FromQuery] string[] userIds,
            string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false, CancellationToken cancellationToken = default)
        {
            userIds ??= new[] { _currentUserService.UserId };
            if (userIds.Length != 1 || userIds[0] != _currentUserService.UserId) // if a user filter is applied that is different than current user, only allowed by admins
                await Get<IPermissionService>().EnsureRoleAsync(RoleConstants.AdministratorRole);
            var data = await _auditService.ExportAsync(exportServiceType, userIds, searchString, searchInOldValues, searchInNewValues, cancellationToken);
            var mimeType = MimeGuesser.GuessMimeType(data);
            var fileDownloadName = $"{ControllerContext.ActionDescriptor.ControllerName}-{DateTime.Now:ddMMyyyyHHmmss}.{MimeTypesMap.GetExtension(mimeType)}";
            return File(data, mimeType, fileDownloadName);
        }
    }
}