using Coworkee.Application.Common.Models.Identity;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Validators.Requests.Identity
{
    public class RoleRequestValidator : AbstractValidator<RoleDto>
    {
        public RoleRequestValidator(IStringLocalizer<RoleRequestValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required"]);
        }
    }
}
