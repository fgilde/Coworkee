using System.Collections.Generic;
using System.Threading;
using CleanArchitectureBase.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Responses.Audit;
using CleanArchitectureBase.Shared.Constants.Permission;
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
        public async Task<IActionResult> GetUserTrailsAsync(CancellationToken cancellationToken = default)
        {
            return Ok(await _auditService.GetCurrentUserTrailsAsync(_currentUserService.UserId));
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
        public async Task<IActionResult> ExportExcel(string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false)
        {
            var data = await _auditService.ExportToExcelAsync(_currentUserService.UserId, searchString, searchInOldValues, searchInNewValues);
            return Ok(data);
        }
    }
}