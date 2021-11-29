using System;
using System.IO;
using CleanArchitectureBase.Infrastructure.Models.Identity;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CleanArchitectureBase.Application;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Contracts.Attributes;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Contracts.Services.Account;
using CleanArchitectureBase.Application.Hubs.Events;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Infrastructure.Services.Identity
{
    [RegisterAs(typeof(IAccountService), 3)]
    public class AccountService : IAccountService
    {
        private readonly IdentityService _identityService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AccountService> _localizer;
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;
        private readonly IAuthorizationService _authorizationService;


        public AccountService(
            IdentityService identityService,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUploadService uploadService,
            IStringLocalizer<AccountService> localizer, 
            IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory,
            IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor,
            IMediator mediator)
        {
            _identityService = identityService;
            _userManager = userManager;
            _signInManager = signInManager;
            _uploadService = uploadService;
            _localizer = localizer;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _authorizationService = authorizationService;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId)
        {
            var user = await this._userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await Result.FailAsync(_localizer["User Not Found."]);
            }

            var identityResult = await _userManager.ChangePasswordAsync(
                user,
                model.Password,
                model.NewPassword);
            var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
            return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
        }

        /// <summary>
        /// Updates the profile and returns the new JWT token
        /// </summary>
        public async Task<string> UpdateProfileAsync(UpdateProfileRequest request, string userId)
        {
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                if (await _userManager.Users.AnyAsync(x => x.PhoneNumber == request.PhoneNumber))
                    throw Errors.Create(_localizer["Phone number {0} is already used.", request.PhoneNumber], HttpStatusCode.Conflict);
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null || userWithSameEmail.Id == userId)
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw Errors.NotFound("User Not found");
                }
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.PhoneNumber = request.PhoneNumber;
                var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
                if (request.PhoneNumber != phoneNumber)
                    await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber).EnsureSuccess();

                await _userManager.UpdateAsync(user).EnsureSuccess();
                await _signInManager.RefreshSignInAsync(user);
                await _mediator.PublishClientEvent(new UserProfileChanged(user.MapTo<UserResponse>()));
                return await _identityService.GenerateJwtAsync(user);

            }
            throw Errors.Create(_localizer["Email {0} is already used.", request.Email], HttpStatusCode.Conflict);
        }

        public async Task<IResult<string>> GetProfilePictureAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await Result<string>.FailAsync(_localizer["User Not Found"]);
            }
            return await Result<string>.SuccessAsync(data: user.ProfilePictureDataUrl);
        }

        public async Task<IResult<TokenResponse>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return await Result<TokenResponse>.FailAsync(message: _localizer["User Not Found"]);
            if (File.Exists(user.ProfilePictureDataUrl))
                File.Delete(user.ProfilePictureDataUrl);
            var filePath = _uploadService.UploadAsync(request);
            user.ProfilePictureDataUrl = filePath;
            var identityResult = await _userManager.UpdateAsync(user);
            var errors = identityResult.Errors.Select(e => _localizer[e.Description].ToString()).ToList();
            if (identityResult.Succeeded)
                await _mediator.PublishClientEvent(new UserProfileChanged(user.MapTo<UserResponse>()));
            return identityResult.Succeeded 
                ? await Result<TokenResponse>.SuccessAsync(new TokenResponse()
                {
                    Token = await _identityService.GenerateJwtAsync(user),
                    UserImageURL = filePath,
                }) 
                : await Result<TokenResponse>.FailAsync(errors);
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            return user.UserName;
        }
        public async Task<(IResult Result, string UserId)> CreateUserAsync(string userName, string password)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = userName,
            };

            var result = await _userManager.CreateAsync(user, password);

            return (result.ToApplicationResult(), user.Id);
        }

        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AuthorizeAsync(string userId, string policyName)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            var result = await _authorizationService.AuthorizeAsync(principal, policyName);

            return result.Succeeded;
        }

        public async Task<IResult> DeleteUserAsync(string userId)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
            {
                return await DeleteUserAsync(user);
            }

            return Result.Success();
        }

        public async Task LogoutAsync()
        {
            try
            {
                _httpContextAccessor?.HttpContext?.Session.Clear();
                await _signInManager.SignOutAsync();
            }
            catch
            {
                // ignored
            }
        }

        public async Task<IResult> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }
    }
}