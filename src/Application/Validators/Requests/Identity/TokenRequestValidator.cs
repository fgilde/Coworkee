using Coworkee.Application.Configurations;
using Coworkee.Application.Requests.Identity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Validators.Requests.Identity
{
    public class TokenRequestValidator : AbstractValidator<TokenRequest>
    {
        public TokenRequestValidator(IStringLocalizer<TokenRequestValidator> localizer, Publicsettings config)
        {
            var emailRule = RuleFor(request => request.Email)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Email is required"]);
            if(!(config?.LoginSettings?.AllowLoginWithUsername ?? false))
                emailRule.EmailAddress().WithMessage(x => localizer["Email is not correct"]);
            
            RuleFor(request => request.Password)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Password is required!"]);
        }
    }
}
