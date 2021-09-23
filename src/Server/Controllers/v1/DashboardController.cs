using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.Dashboards.Queries.GetData;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Http;

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
        [Produces(typeof(Result<DashboardDataResponse>))]
        public async Task<IActionResult> GetDataAsync()
        {
            return Ok(await _mediator.Send(new GetDashboardDataQuery()));
        }

        /// <summary>
        /// Get the new ultimate Dashboard Data
        /// </summary>
        /// <returns>Status 200 OK </returns>
        [Authorize(Policy = Permissions.Hangfire.View)]
        [HttpGet("jobdashboardurl")]
        [Produces(typeof(string))]
        public IActionResult GetJobDashBoardUrl()
        {
            HttpContext.Session.SetString(ApplicationConstants.Hangfire.SessionUserIdKey, Get<ICurrentUserService>().UserId);
            return Ok(ApplicationConstants.Hangfire.DashboardRoute);
        }
    }
}