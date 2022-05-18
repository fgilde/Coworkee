using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application;
using CleanArchitectureBase.Application.Common.Exceptions;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Attributes;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Contracts.Services.ExportImport;
using CleanArchitectureBase.Application.Contracts.Services.Identity;
using CleanArchitectureBase.Application.Hubs.Events.Base;
using CleanArchitectureBase.Application.Requests.Identity;
using CleanArchitectureBase.Application.Requests.Mail;
using CleanArchitectureBase.Infrastructure.Contexts;
using CleanArchitectureBase.Infrastructure.Helpers;
using CleanArchitectureBase.Infrastructure.Models.Identity;
using CleanArchitectureBase.Infrastructure.Specifications;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Constants.Permission;
using CleanArchitectureBase.Shared.Constants.Role;
using CleanArchitectureBase.Shared.Models;
using CleanArchitectureBase.Shared.Wrapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Application.Requests;

namespace CleanArchitectureBase.Infrastructure.Services.Identity
{
    [RegisterAs(typeof(IUserService), 5)]
    public class UserService : IUserService
    {
        private readonly IPermissionService _permissionService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMailService _mailService;
        private readonly IStringLocalizer<UserService> _localizer;
        private readonly IExportService _excelService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApplicationDbContext _db;

        public UserService(
            IPermissionService permissionService,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IMailService mailService,
            IStringLocalizer<UserService> localizer,
            ICurrentUserService currentUserService,
            ApplicationDbContext db,
            IServiceProvider serviceProvider)
        {
            _permissionService = permissionService;
            _userManager = userManager;
            _roleManager = roleManager;
            _mailService = mailService;
            _localizer = localizer;
            _excelService = serviceProvider.GetServices<IExportService>().FirstOrDefault(s => s.ExportService == ExportServiceType.Excel);
            _currentUserService = currentUserService;
            _serviceProvider = serviceProvider;
            _db = db;
        }

        public async Task<IEnumerable<UserResponse>> GetAllForTargetAsync(EventTarget eventTarget)
        {
            var all = await GetAllAsync();
            if (eventTarget == EventTarget.All)
                return all.Data;
            if (eventTarget == EventTarget.Current)
                return all.Data.Where(r => r.Id == _currentUserService.UserId);
            if (eventTarget.Key == nameof(EventTarget.User))
                return all.Data.Where(r => eventTarget.Groups.Contains(r.Id));
            if (eventTarget.Key == nameof(EventTarget.WithRole))
                return all.Data.Where(r => GetRolesAsync(r.Id).Result.Data.UserRoles.Where(r => r.Selected).Any(role => eventTarget.Groups.Contains(role.Id) || eventTarget.Groups.Contains(role.RoleName)));
            if (eventTarget.Key == nameof(EventTarget.WithPermission))
                return all.Data.Where(r => _permissionService.HasPoliciesAsync(eventTarget.Groups, PolicyMatch.Any, r.Id).Result);

            return Enumerable.Empty<UserResponse>();
        }

        public async Task<UserResponse> SystemUserAsync()
        {
            var res = await GetOrAddUserAsync(ApplicationConstants.Defaults.Users.System);
            return res.First();
        }

        public Task<UserResponse[]> GetOrAddUserAsync(params CreateUser[] userToCreateIfNotExists)
        {
            return Task.Run(async () =>
            {
                var result = new List<UserResponse>();
                bool needSave = false;
                foreach (var u in userToCreateIfNotExists)
                {
                    //Check if Role Exists
                    var targetRoleName = u.RoleToAdd;
                    var applicationRole = new ApplicationRole(targetRoleName, _localizer[targetRoleName + " role " + (u.IsSuperUser ? "with full permissions" : "with default permissions")]);
                    var roleInDb = await _roleManager.FindByNameAsync(targetRoleName);
                    if (roleInDb == null)
                    {
                        await _roleManager.CreateAsync(applicationRole);
                        roleInDb = await _roleManager.FindByNameAsync(targetRoleName);
                        needSave = true;
                    }

                    var user = u.MapTo<ApplicationUser>();
                    user.EmailConfirmed = true;
                    user.PhoneNumberConfirmed = true;
                    user.CreatedOn = DateTime.Now;
                    user.IsActive = true;
                    //Check if User Exists

                    var userInDb = await _userManager.FindByEmailAsync(user.Email);
                    if (userInDb == null)
                    {
                        await _userManager.CreateAsync(user, u.Password);
                        await _userManager.AddToRoleAsync(user, targetRoleName);
                        needSave = true;
                        userInDb = await _userManager.FindByEmailAsync(user.Email);
                    }

                    if (u.IsSuperUser)
                    {
                        foreach (var permission in Permissions.GetRegisteredPermissions())
                        {
                            await _roleManager.AddPermissionClaim(roleInDb, permission);
                        }
                    }
                    result.Add((await GetAsync(userInDb.Id)).Data);
                }

                if (needSave)
                    await _db.SaveChangesAsync();
                return result.ToArray();
            });
        }

        public async Task<Result<List<UserResponse>>> GetAllAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = users.MapTo<List<UserResponse>>().Where(u => !u.IsSystemUser()).ToList();
            return await Result<List<UserResponse>>.SuccessAsync(result);
        }

        public async Task<IResult> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                return await Result.FailAsync(string.Format(_localizer["Username {0} is already taken."], request.UserName));
            }
            var user = new ApplicationUser
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                IsActive = request.ActivateUser,
                EmailConfirmed = request.AutoConfirmEmail
            };

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
                if (userWithSamePhoneNumber != null)
                {
                    return await Result.FailAsync(string.Format(_localizer["Phone number {0} is already registered."], request.PhoneNumber));
                }
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, RoleConstants.BasicRole);
                    if (!request.AutoConfirmEmail)
                        await SendVerificationMailAsync(origin, user);
                    if (user.EmailConfirmed && user.IsActive)
                        SendUserActivatedMailAsync(user);

                    var message = string.Format(request.AutoConfirmEmail ? _localizer["User {0} Registered."] : _localizer["User {0} Registered. Please check your Mailbox to verify!"], user.UserName);
                    return await Result<string>.SuccessAsync(user.Id, message);
                }

                return await Result.FailAsync(result.Errors.Select(a => _localizer[a.Description].ToString()).ToList());
            }

            return await Result.FailAsync(string.Format(_localizer["Email {0} is already registered."], request.Email));
        }

        private void SendUserActivatedMailAsync(ApplicationUser user)
        {
            var url = _serviceProvider.GetService<ServerConfiguration>()?.ClientUrl;
            var body = string.Format(_localizer["Your Account is confirmed and active, you can now Login"], url);
            if (!string.IsNullOrEmpty(url))
                body += $"<a href='{url.EnsureEndsWith("/")}{ApplicationConstants.Routes.Login}?email={user.Email}'> Login to {ApplicationConstants.ApplicationName} </a>";

            var mailRequest = new MailRequest
            {
                RecipientName = $"{user.FirstName} {user.LastName}",
                To = user.Email,
                Body = body,
                Subject = $"{ApplicationConstants.ApplicationName} - " + _localizer["Your Account is activated "]
            };
            BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
        }

        private async Task SendVerificationMailAsync(string origin, ApplicationUser user)
        {
            var verificationUri = await GetVerificationUriAsync(user, origin);
            var mailRequest = new MailRequest
            {
                RecipientName = $"{user.FirstName} {user.LastName}",
                To = user.Email,
                Body = string.Format(_localizer["Please confirm your account by <a href='{0}'>clicking here</a>."],
                    verificationUri),
                Subject = _localizer["Confirm Registration"]
            };
            BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
        }

        private async Task<string> GetVerificationUriAsync(ApplicationUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/v1/identity/user/confirm-email/";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            return verificationUri;
        }

        public UserResponse Get(string userId)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == userId);
            return user?.MapTo<UserResponse>();
        }

        public async Task<IResult<UserResponse>> GetAsync(string userId)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return await Result<UserResponse>.FailAsync($"User with id {userId} not found");
            return await Result<UserResponse>.SuccessAsync(user.MapTo<UserResponse>());
        }

        public async Task<IResult> ToggleUserStatusAsync(ToggleUserStatusRequest request)
        {
            var user = await _userManager.Users.Where(u => u.Id == request.UserId).FirstOrDefaultAsync();
            var isAdmin = await _userManager.IsInRoleAsync(user, RoleConstants.AdministratorRole);
            if (isAdmin)
            {
                return await Result.FailAsync(_localizer["Administrators Profile's Status cannot be toggled"]);
            }
            if (user != null)
            {
                user.IsActive = request.ActivateUser;
                user.EmailConfirmed = request.EmailConfirmed;
                var identityResult = await _userManager.UpdateAsync(user);
                if (user.EmailConfirmed && user.IsActive)
                    SendUserActivatedMailAsync(user);
                return identityResult.ToApplicationResult();
            }
            return await Result.SuccessAsync();
        }

        public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId = null)
        {
            bool selectedOnly = userId == null;
            userId ??= _currentUserService.UserId;
            var viewModel = new List<UserRoleModel>();
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var userRolesViewModel = new UserRoleModel
                {
                    Id = role.Id,
                    RoleName = role.Name,
                    RoleDescription = role.Description
                };
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.Selected = true;
                }
                else
                {
                    userRolesViewModel.Selected = false;
                }
                if (!selectedOnly || userRolesViewModel.Selected)
                    viewModel.Add(userRolesViewModel);
            }
            var result = new UserRolesResponse { UserRoles = viewModel };
            return await Result<UserRolesResponse>.SuccessAsync(result);
        }

        public async Task<IResult> UpdateRolesAsync(UpdateUserRolesRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user.Email == ApplicationConstants.Defaults.Users.System.Email || ApplicationConstants.Defaults.Users.Administrators.Any(u => u.Email == user.Email))
            {
                return await Result.FailAsync(_localizer["Not Allowed."]);
            }

            var roles = await _userManager.GetRolesAsync(user);
            var selectedRoles = request.UserRoles.Where(x => x.Selected).ToList();

            var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
            if (!await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
            {
                var tryToAddAdministratorRole = selectedRoles
                    .Any(x => x.RoleName == RoleConstants.AdministratorRole);
                var userHasAdministratorRole = roles.Any(x => x == RoleConstants.AdministratorRole);
                if (tryToAddAdministratorRole && !userHasAdministratorRole || !tryToAddAdministratorRole && userHasAdministratorRole)
                {
                    return await Result.FailAsync(_localizer["Not Allowed to add or delete Administrator Role if you have not this role."]);
                }
            }

            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            result = await _userManager.AddToRolesAsync(user, selectedRoles.Select(y => y.RoleName));
            return await Result.SuccessAsync(_localizer["Roles Updated"]);
        }

        public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                if (!user.IsActive)
                    await SendAdminActivationNotification(user);
                if (user.EmailConfirmed && user.IsActive)
                    SendUserActivatedMailAsync(user);
                return await Result<string>.SuccessAsync(user.Id, string.Format(_localizer["Account Confirmed for {0}. You can now use the /api/identity/token endpoint to generate JWT."], user.Email));
            }

            throw new ApiException(string.Format(_localizer["An error occurred while confirming {0}"], user.Email));
        }

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return await Result.FailAsync(_localizer["An Error has occurred!"]);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                await SendVerificationMailAsync(origin, user);

                return await Result.SuccessAsync(string.Format(_localizer["User {0} Registered. Please check your Mailbox to verify!"], user.UserName));
            }
            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "account/reset-password";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var passwordResetURL = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
            var mailRequest = new MailRequest
            {
                RecipientName = $"{user.FirstName} {user.LastName}",
                Body = string.Format(_localizer["Please reset your password by <a href='{0}'>clicking here</a>."], HtmlEncoder.Default.Encode(passwordResetURL)),
                Subject = _localizer["Reset Password"],
                To = request.Email
            };
            BackgroundJob.Enqueue(() => _mailService.SendAsync(mailRequest));
            return await Result.SuccessAsync(_localizer["Password Reset Mail has been sent to your authorized Email."]);
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return await Result.FailAsync(_localizer["An Error has occurred!"]);
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (result.Succeeded)
            {
                return await Result.SuccessAsync(_localizer["Password Reset Successful!"]);
            }
            else
            {
                return await Result.FailAsync(_localizer["An Error has occurred!"]);
            }
        }

        public async Task<int> GetCountAsync()
        {
            var count = await _userManager.Users.CountAsync();
            return count - 1; // We removing the system user here
        }

        public async Task<string> ExportToExcelAsync(string searchString = "")
        {
            var userSpec = new UserFilterSpecification(searchString);
            var users = await _userManager.Users
                .Specify(userSpec)
                .OrderByDescending(a => a.CreatedOn)
                .ToListAsync();
            var result = await _excelService.ExportAsync(users);

            return Convert.ToBase64String(result);
        }

        public async Task<IResult> DeleteAsync(string userId, CancellationToken cancellationToken = default)
        {
            if (_currentUserService.UserId == userId)
                throw Errors.Create("You cannot delete yourself");
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw Errors.NotFound(_localizer["User Not Found!"]);
            if (user.IsSystemUser())
                throw Errors.Create("Not allowed to Delete this user");
            var result = await _userManager.DeleteAsync(user);
            return result.ToApplicationResult();
        }

        private Task SendAdminActivationNotification(ApplicationUser user)
        {
            return _serviceProvider.GetService<INotificationService>()?.SendAsync(new NotificationRequest
            {
                PersistInDb = true,
                SendAsMail = NotificationAsMail.Always,
                Subject = $"{ApplicationConstants.ApplicationName} - New User with valid Email registered",
                Content = $"The User '{user.FirstName} {user.LastName}' has recently confirmed his email address '{user.Email}' and is waiting for activation.",
                Url = $"/user-profile/{user.Id}",
                Target = EventTarget.WithRole(RoleConstants.AdministratorRole)
            });
        }
    }
}