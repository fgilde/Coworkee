using System.IO;
using Coworkee.Infrastructure.Models.Identity;
using Coworkee.Application.Requests.Identity;
using Coworkee.Shared.Wrapper;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Coworkee.Application;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Attributes;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Contracts.Services.Account;
using Coworkee.Application.Hubs.Events;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;

namespace Coworkee.Infrastructure.Services.Identity
{
    [RegisterAs(typeof(IAccountService), 3, ServiceLifetime = ServiceLifetime.Scoped)]
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
        private string[] _temporaryRoles;
        private string[] _temporaryPermissions;

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

        public async Task<IResult<string>> GetProfilePictureAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return await Result<string>.FailAsync(_localizer["User Not Found"]);

            return await Result<string>.SuccessAsync(data: user.ProfilePictureDataUrl);
        }

        public async Task<IResult<TokenResponse>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return await Result<TokenResponse>.FailAsync(message: _localizer["User Not Found"]);
            if (File.Exists(user.ProfilePictureDataUrl))
                File.Delete(user.ProfilePictureDataUrl);
            var filePath = await _uploadService.UploadAsync(request);
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
            if (_temporaryRoles?.Contains(role) == true)
                return true;

            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AuthorizeAsync(string userId, string policyName)
        {
            if (_temporaryPermissions?.Contains(policyName) == true)
                return true;

            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);
            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
            var result = await _authorizationService.AuthorizeAsync(principal, policyName);

            return result.Succeeded;
        }

        public async Task<IResult> DeleteUserAsync(string userId)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
                return await DeleteUserAsync(user);

            return await Result.SuccessAsync();
        }

        public Task WithRoles(params string[] roles)
        {
            _temporaryRoles = roles;
            return Task.CompletedTask;
        }

        public Task WithPermissions(params string[] permissions)
        {
            _temporaryPermissions = permissions;
            return Task.CompletedTask;
        }

        public async Task LogoutAsync()
        {
            try
            {
                _httpContextAccessor?.HttpContext?.Session.Clear();
                await _signInManager.SignOutAsync();
            }
            catch {/*ingnored*/}
        }

        public async Task<IResult> DeleteUserAsync(ApplicationUser user)
        {
            if (user.IsSystemUser())
                throw Errors.Create("Not allowed to Delete this user");
            var result = await _userManager.DeleteAsync(user);
            return result.ToApplicationResult();
        }
    }
}