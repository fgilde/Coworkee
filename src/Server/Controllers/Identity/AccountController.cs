using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using Coworkee.Application.Requests.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Contracts.Services.Account;
using Coworkee.Infrastructure.Services.Identity;
using Coworkee.Shared.Wrapper;
using Coworkee.Application.Contracts.Services.Identity;
using Coworkee.Application.Common.Models;
using Coworkee.Server.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Coworkee.Shared.Constants.Application;
using Microsoft.AspNetCore.Http;
using Coworkee.Application.Configurations;
using Coworkee.Infrastructure.Models.Identity;
using Coworkee.Shared;
using Microsoft.IdentityModel.JsonWebTokens;
using Nextended.Core.Extensions;
using System.Net.Http;

namespace Coworkee.Server.Controllers.Identity
{
    [Authorize]
    [ApiController]
    [Route("api/v{version:apiVersion}/identity/[controller]")]

    public class AccountController : BaseApiController<AccountController>
    {
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUser;

        public AccountController(IAccountService accountService, ICurrentUserService currentUser)
        {
            _accountService = accountService;
            _currentUser = currentUser;
        }
        
        /// <summary>
        /// Change Password
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPut(nameof(ChangePassword))]
        [Produces(typeof(Result))]
        public async Task<ActionResult> ChangePassword(ChangePasswordRequest model)
        {
            var response = await _accountService.ChangePasswordAsync(model, _currentUser.UserId);
            return Ok(response);
        }

        /// <summary>
        /// Get Profile picture by Id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Status 200 OK </returns>
        [HttpGet("profile-picture/{userId}")]
        [ResponseCache(NoStore = false, Location = ResponseCacheLocation.Client, Duration = 60)]
        [Produces(typeof(Result<string>))]
        public async Task<IActionResult> GetProfilePictureAsync(string userId)
        {
            return Ok(await _accountService.GetProfilePictureAsync(userId));
        }

        /// <summary>
        /// Update Profile Picture
        /// </summary>
        /// <param name="request"></param>
        /// <param name="userId"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost("profile-picture/{userId}")]
        [Produces(typeof(Result<TokenResponse>))]
        public async Task<IActionResult> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId)
        {
            return Ok(await _accountService.UpdateProfilePictureAsync(request, string.IsNullOrWhiteSpace(userId) ? _currentUser.UserId : userId));
        }

        /// <summary>
        /// SetPreferredLanguage
        /// </summary>
        /// <param name="language"></param>
        /// <returns>Status 200 OK</returns>
        [HttpPost(nameof(SetPreferredLanguage))]
        [Produces(typeof(Result<Result>))]
        public async Task<IActionResult> SetPreferredLanguage(LanguageDto language)
        {
            if (!string.IsNullOrEmpty(_currentUser.UserId) && language != null)
            {
                return Ok(await Get<IUserService>().SetUserCulture(_currentUser.UserId, language.ToCulture()));
            }
            return Ok();
        }

        [HttpGet("login-callback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var token = HttpContext?.Session?.GetString(ApplicationConstants.ParameterNames.AuthedUrlParameter);
            var claims = ClaimReader.ReadClaimsFromJwt(token);
            var userId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            await Get<IdentityService>().SetUserOnlineStatusAsync(userId, true);

            // Ensure returnUrl is not null or empty
            returnUrl ??= "/";

            // Check if returnUrl already has query parameters
            string separator = returnUrl.Contains("?") ? "&" : "?";

            // Append the parameter safely
            string redirectUrl = $"{returnUrl}{separator}{ApplicationConstants.ParameterNames.AuthedUrlParameter}={Uri.EscapeDataString(token)}";

            return Redirect(redirectUrl);
        }



        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = "/")
        {
            var callbackUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl }, Request.Scheme);

            return Challenge(new AuthenticationProperties
            {
                RedirectUri = callbackUrl
            }, "keycloak");
        }

        [HttpPost("[action]")]
        [AllowAnonymous] // To ensure no error if call comes with expired session
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            await Get<IdentityService>().SetUserOnlineStatusAsync(_currentUser.UserId, false);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext?.Session?.SetString(ApplicationConstants.ParameterNames.AuthedUrlParameter, "");

            // Keycloak logout
            //var keycloakLogoutUrl = $"{Configuration.KeycloakConfiguration.Url}/realms/{Configuration.KeycloakConfiguration.Realm}/protocol/openid-connect/logout?redirect_uri={Configuration.ClientUrl.EnsureEndsWith("/")}";

            return Ok();
        }
    }
}