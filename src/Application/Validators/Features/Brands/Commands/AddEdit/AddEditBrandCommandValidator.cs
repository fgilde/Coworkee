using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Features.Brands.Commands.AddEdit;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Validators.Features.Brands.Commands.AddEdit
{
    public class AddEditBrandsCommandValidator : AbstractValidator<AddEditBrandsCommand>
    {
        public AddEditBrandsCommandValidator(IStringLocalizer<AddEditBrandCommandValidator> localizer)
        {
            RuleForEach(x => x.Items).SetValidator(new AddEditBrandCommandValidator(localizer));
        }
    }
    public class AddEditBrandCommandValidator : AbstractValidator<BrandDto>
    {
        public AddEditBrandCommandValidator(IStringLocalizer<AddEditBrandCommandValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);
            RuleFor(request => request.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);
            //RuleFor(request => request.Tax)
            //    .GreaterThan(0).WithMessage(x => localizer["Tax must be greater than 0"]);
        }
    }
}