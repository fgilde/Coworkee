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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

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

        [HttpGet("login")]
        public async Task<IActionResult> Login(string returnUrl = "/")
        {
            await HttpContext.ChallengeAsync("keycloak", new OAuthChallengeProperties() { RedirectUri = returnUrl });
            return Ok();
        }

        //[HttpGet("login")]
        //public IActionResult Login(string returnUrl = "/")
        //{
        //    return Challenge(new AuthenticationProperties
        //    {
        //        RedirectUri = returnUrl
        //    }, "keycloak");
        //}

        [HttpPost("[action]")]
        [AllowAnonymous] // To ensure no error if call comes with expired session
        public async Task<IActionResult> Logout()
        {
            await _accountService.LogoutAsync();
            await Get<IdentityService>().SetUserOnlineStatusAsync(_currentUser.UserId, false);
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok();
        }
    }
}