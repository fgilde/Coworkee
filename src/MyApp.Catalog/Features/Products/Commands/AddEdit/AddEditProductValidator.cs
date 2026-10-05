using FluentValidation;

namespace MyApp.Catalog.Features.Products.Commands.AddEdit;

internal sealed class AddEditProductValidator : AbstractValidator<AddEditProductCommand>
{
    private const int MaxImageLength = 2 * 1024 * 1024;

    public AddEditProductValidator()
    {
        RuleFor(c => c.Product.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Product.Barcode).MaximumLength(100);
        RuleFor(c => c.Product.Description).MaximumLength(2000);
        RuleFor(c => c.Product.Rate).GreaterThanOrEqualTo(0);
        RuleFor(c => c.Product.BrandId).NotEmpty();
        RuleFor(c => c.Product.ImageDataUrl)
            .Must(url => url is null || url.StartsWith("data:image/", StringComparison.Ordinal))
            .WithMessage("The image must be a data URL of an image.")
            .MaximumLength(MaxImageLength);
    }
}
