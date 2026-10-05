using FluentValidation;

namespace MyApp.Catalog.Features.Brands.Commands.AddEdit;

internal sealed class AddEditBrandValidator : AbstractValidator<AddEditBrandCommand>
{
    public AddEditBrandValidator()
    {
        RuleFor(c => c.Brand.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Brand.Description).MaximumLength(2000);
        RuleFor(c => c.Brand.Tax).InclusiveBetween(0, 100);
    }
}
