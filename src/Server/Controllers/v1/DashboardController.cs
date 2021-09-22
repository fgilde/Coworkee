using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.Dashboards.Queries.GetData;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Constants.Permission;
using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using Nextended.Core.Encryption;
using Nextended.Core.Hashing;

namespace CleanArchitectureBase.Server.Controllers.v1
{
    [ApiController]
    public class DashboardController : BaseApiController<DashboardController>
    {
        /// <summary>
        /// Get the new ultimate Dashboard Data
        /// </summary>
        /// <returns>Status 200 OK </returns>
        [Authorize(Policy = Permissions.Dashboards.View)]
        [HttpGet]
        public async Task<IActionResult> GetDataAsync()
        {
            var result = await _mediator.Send(new GetDashboardDataQuery());
            return Ok(result);
        }

        /// <summary>
        /// Get the new ultimate Dashboard Data
        /// </summary>
        /// <returns>Status 200 OK </returns>
        [Authorize(Policy = Permissions.Hangfire.View)]
        [HttpGet("jobdashboardurl")]
        public async Task<IActionResult> GetJobDashBoardUrl()
        {
            HttpContext.Session.SetString(ApplicationConstants.Hangfire.SessionUserIdKey, Get<ICurrentUserService>().UserId);
            return Ok(ApplicationConstants.Hangfire.DashboardRoute);
        }
    }
}