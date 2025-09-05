using System.Threading.Tasks;
using lib.Coworkee.Application.Features.Dashboards.Queries.GetData;
using lib.Coworkee.Shared.Constants.Permission;
using lib.Coworkee.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Coworkee.Server.Controllers
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
            return Ok(await Mediator.Send(new GetDashboardDataQuery()));
        }
    }
}