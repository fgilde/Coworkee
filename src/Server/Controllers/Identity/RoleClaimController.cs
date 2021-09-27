using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Interfaces.Services.Identity;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Application.Responses.Identity;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitectureBase.Server.Controllers.Identity
{
    [ApiController]
    [Route("api/v{version:apiVersion}/identity/[controller]")]

    public class RoleClaimController : BaseApiController<RoleClaimController>
    {
        private readonly IRoleClaimService _roleClaimService;

        public RoleClaimController(IRoleClaimService roleClaimService)
        {
            _roleClaimService = roleClaimService;
        }

        /// <summary>
        /// Get All Role Claims(e.g. Product Create Permission)
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.View)]
        [HttpGet]
        [Produces(typeof(Result<List<RoleClaimResponse>>))]
        public async Task<IActionResult> GetAll()
        {
            var roleClaims = await _roleClaimService.GetAllAsync();
            return Ok(roleClaims);
        }

        /// <summary>
        /// Get All Role Claims By Id
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.View)]
        [HttpGet("{roleId}")]
        [Produces(typeof(Result<List<RoleClaimResponse>>))]
        public async Task<IActionResult> GetAllByRoleId([FromRoute] string roleId)
        {
            var response = await _roleClaimService.GetAllByRoleIdAsync(roleId);
            return Ok(response);
        }

        /// <summary>
        /// Add a Role Claim
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK </returns>
        [Authorize(Policy = Permissions.RoleClaims.Create)]
        [HttpPost]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> Post(RoleClaimRequest request)
        {
            var response = await _roleClaimService.SaveAsync(request);
            return Ok(response);
        }

        /// <summary>
        /// Delete a Role Claim
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.RoleClaims.Delete)]
        [HttpDelete("{id}")]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _roleClaimService.DeleteAsync(id);
            return Ok(response);
        }
    }
}