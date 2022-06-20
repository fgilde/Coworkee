using System;
using System.Linq;
using CleanArchitectureBase.Application.Requests.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Contracts.Services.Identity;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitectureBase.Server.Controllers.Identity
{
    [ApiController]
    [Route("api/v{version:apiVersion}/identity/[controller]")]

    public class TokenController : BaseApiController<RoleController>
    {
        private readonly ITokenService _identityService;

        public TokenController(ITokenService identityService)
        {
            _identityService = identityService;
        }

        /// <summary>
        /// Regenerates a new token.
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [HttpPost(nameof(RegenerateNew))]
        [Authorize]
        [Produces(typeof(Result<TokenResponse>))]
        public async Task<ActionResult> RegenerateNew()
        {
            string[] roleIds = Request.Headers.TryGetValue(ApplicationConstants.HeaderNames.RoleIdHeader, out var idValues) ? idValues.SelectMany(s => s.Split(',')).Select(s => s.Trim()).ToArray() : Array.Empty<string>();
            var response = await _identityService.RegenerateTokenAsync(roleIds);
            return Ok(response);
        }

        /// <summary>
        /// Get Token (Email, Password)
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost]
        [Produces(typeof(Result<TokenResponse>))]
        public async Task<ActionResult> Get(TokenRequest model)
        {
            var response = await _identityService.LoginAsync(model);
            return Ok(response);
        }

        /// <summary>
        /// Refresh Token
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("refresh")]
        [Produces(typeof(Result<TokenResponse>))]
        public async Task<ActionResult> Refresh([FromBody] RefreshTokenRequest model)
        {
            var response = await _identityService.GetRefreshTokenAsync(model);
            return Ok(response);
        }
    }
}