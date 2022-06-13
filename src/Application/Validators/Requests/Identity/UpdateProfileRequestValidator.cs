using FluentValidation;
using Microsoft.Extensions.Localization;
using CleanArchitectureBase.Application.Common.Models.Identity;
using CleanArchitectureBase.Application.Configurations;

namespace CleanArchitectureBase.Application.Validators.Requests.Identity
{
    public class UpdateProfileValidator : AbstractValidator<UserResponse>
    {
        public UpdateProfileValidator(IStringLocalizer<UpdateProfileValidator> localizer, Publicsettings config)
        {
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
        }
    }
}