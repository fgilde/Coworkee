using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Coworkee.Application.Requests.Identity;
using Coworkee.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Nextended.Core.Extensions;
using Coworkee.Application;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Services.Identity;
using Coworkee.Server.Extensions;
using Coworkee.Server.Middlewares;
using Coworkee.Shared;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Wrapper;

namespace Coworkee.Server.Controllers.Identity
{
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/identity/[controller]")]
    public class UserController : BaseApiController<UserController>
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Update profile
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost(nameof(UpdateProfile))]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> UpdateProfile(UserResponse user)
        {
            return Ok(await _userService.UpdateUserAsync(user));
        }

        /// <summary>
        /// Deletes a user by id
        /// </summary>
        /// <param name="userId"></param>
        [Authorize(Policy = Permissions.Users.Delete)]
        [HttpDelete]
        [Produces(typeof(Result))]
        public async Task<IActionResult> Delete(string userId, CancellationToken cancellationToken = default)
        {
            return Ok(await _userService.DeleteAsync(userId, cancellationToken));
        }

        /// <summary>
        /// Get User Details
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet]
        [Produces(typeof(Result<List<UserResponse>>))]
        public async Task<IActionResult> GetAll([FromOdataFilter] TransferableExpression<UserResponse> filter = null)
        {
            var users = await _userService.GetAllAsync();
            users.Data = Filter(users.Data, filter).ToList();

            return Ok(users);
        }

        /// <summary>
        /// Get User By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [HttpGet("{id}")]
        [Produces(typeof(Result<UserResponse>))]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetAsync(id);
            return Ok(user);
        }

        /// <summary>
        /// Get User Roles By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Users.View)]
        [HttpGet("roles/{id}")]
        [Produces(typeof(Result<UserRolesResponse>))]
        public async Task<IActionResult> GetRolesAsync(string id)
        {
            var userRoles = await _userService.GetRolesAsync(id);
            return Ok(userRoles);
        }

        /// <summary>
        /// Get Actual User Roles
        /// </summary>
        /// <returns>Status 200 OK</returns>
        [Authorize]
        [HttpGet("roles/my")]
        [Produces(typeof(Result<UserRolesResponse>))]
        public async Task<IActionResult> GetMyRolesAsync()
        {
            var userRoles = await _userService.GetRolesAsync();
            return Ok(userRoles);
        }

        /// <summary>
        /// Update Roles for User
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Users.Edit)]
        [HttpPut("roles/{id}")]
        [Produces(typeof(Result))]
        public async Task<IActionResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            return Ok(await _userService.UpdateRolesAsync(request));
        }

        /// <summary>
        /// Register a User
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [AllowAnonymous]
        [HttpPost]
        [Produces(typeof(Result))]
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            var origin = Request.Headers["origin"];
            if (!HttpContext.User.HasPolicies(PolicyMatch.All, Permissions.Users.Create, Permissions.Roles.View))
            {
                if (!Configuration.PublicSettings.UserRegistration.Enabled)
                {
                    throw Errors.Create("User registration is not allowed");
                }
                request.IsActive = !Configuration.PublicSettings.UserRegistration.RequiresAdministratorActivation;
                request.EmailConfirmed = !Configuration.PublicSettings.UserRegistration.EmailConfirmationRequired;
                request.InitialRoleNames = (await Get<IRoleService>().GetAllAsync()).Data
                    .Where(r => r.IsSelectableByUser && request.InitialRoleNames.Contains(r.Name)).Select(r => r.Name)
                    .ToList();
            }
            return Ok(await _userService.RegisterAsync(request, origin));
        }

        /// <summary>
        /// Confirm Email
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="code"></param>
        /// <returns>Status 200 OK</returns>
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> ConfirmEmailAsync([FromQuery] string userId, [FromQuery] string code)
        {
            var result = await _userService.ConfirmEmailAsync(userId, code);
            if (!Request.IsAjaxRequest())
            {
                var userResponse = (await _userService.GetAsync(userId)).Data;
                var userMail = userResponse.Email;
                var activated = userResponse.IsActive.ToString().ToLower();
                var url = string.IsNullOrEmpty(Configuration.ClientUrl) ? $"{Request.Scheme}://{Request.Host}" : Configuration.ClientUrl;
                return Redirect($"{url.EnsureEndsWith("/")}{ApplicationConstants.Routes.Login}?email-confirmation-result={result.Succeeded.ToString().ToLower()}&email={userMail}&activated={activated}");
            }
            return Ok(result);
        }

        /// <summary>
        /// Toggle User Status (Activate and Deactivate)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("toggle-status")]
        [Produces(typeof(Result))]
        public async Task<IActionResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            return Ok(await _userService.ToggleUserStatusAsync(request));
        }

        /// <summary>
        /// Forgot Password
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [Produces(typeof(Result))]
        public async Task<IActionResult> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var origin = Request.Headers["origin"];
            return Ok(await _userService.ForgotPasswordAsync(request, origin));
        }

        /// <summary>
        /// Reset Password
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("reset-password")]
        [AllowAnonymous]
        [Produces(typeof(Result))]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return Ok(await _userService.ResetPasswordAsync(request));
        }

        /// <summary>
        /// Export to Excel
        /// </summary>
        /// <param name="searchString"></param>
        /// <returns>Status 200 OK</returns>
        [Authorize(Policy = Permissions.Users.Export)]
        [HttpGet("export")]
        [Produces(typeof(string))]
        public async Task<IActionResult> Export(string searchString = "")
        {
            var data = await _userService.ExportToExcelAsync(searchString);
            return Ok(data);
        }
    }
}