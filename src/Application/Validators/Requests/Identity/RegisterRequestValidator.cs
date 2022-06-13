using System.Linq;
using CleanArchitectureBase.Application.Requests.Identity;
using FluentValidation;
using Microsoft.Extensions.Localization;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Configurations;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Shared.Constants.Permission;

namespace CleanArchitectureBase.Application.Validators.Requests.Identity
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator(IStringLocalizer<RegisterRequestValidator> localizer, Publicsettings config, ICurrentUserService currentUserService)
        // Notice: All injected services must provided by server and client
        {
            if (config.UserRegistration.RequireAddress)
            {
                var canCreateUser = currentUserService?.Principal?.HasPolicy(Permissions.Users.Create) ?? false;
                RuleFor(request => request.UserInfo.Addresses)
                    .Must(x => canCreateUser || x?.Any() == true).WithMessage(x => localizer["At least one address is required"]);
                RuleForEach(x => x.UserInfo.Addresses).SetValidator(new AddressValidator());
            }

            if (config.UserRegistration.RequireDocuments)
            {
                // If you're are logged in and allowed to create user its not a registration process
                var canCreateUser = currentUserService?.Principal?.HasPolicy(Permissions.Users.Create) ?? false;
                RuleFor(request => request.Documents)
                    .Must(list => canCreateUser || list?.Any() == true).WithMessage(localizer["At least one document is required"]);
            }

            RuleFor(request => request.FirstName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["First Name is required"]);
            RuleFor(request => request.LastName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Last Name is required"]);
            RuleFor(request => request.Email)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Email is required"])
                .EmailAddress().WithMessage(x => localizer["Email is not correct"]);
            RuleFor(request => request.UserName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["UserName is required"])
                .MinimumLength(config.UserRegistration.UsernameRules.MinLength).WithMessage(localizer["UserName must be at least of length {0}", config.UserRegistration.UsernameRules.MinLength]);

            var passwordRule = RuleFor(request => request.Password)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password is required!"])
                .MinimumLength(config.UserRegistration.PasswordRules.MinLength).WithMessage(localizer["Password must be at least of length {0}", config.UserRegistration.PasswordRules.MinLength]);
            if (config.UserRegistration.PasswordRules.CapitalLetterRequired)
                passwordRule.Matches(@"[A-Z]").WithMessage(localizer["Password must contain at least one capital letter"]);
            if (config.UserRegistration.PasswordRules.LowercaseLetterRequired)
                passwordRule.Matches(@"[a-z]").WithMessage(localizer["Password must contain at least one lowercase letter"]);
            if (config.UserRegistration.PasswordRules.NumberRequired)
                passwordRule.Matches(@"[0-9]").WithMessage(localizer["Password must contain at least one digit"]);

            RuleFor(request => request.ConfirmPassword)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password Confirmation is required!"])
                .Equal(request => request.Password).WithMessage(x => localizer["Passwords don't match"]);
        }
    }
}