using System.Linq;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.Products.Commands.AddEdit;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Validators.Features.Products.Commands.AddEdit
{

    public class AddEditProductsCommandValidator : AbstractValidator<AddEditProductsCommand>
    {
        public AddEditProductsCommandValidator(IStringLocalizer<AddEditProductCommandValidator> localizer)
        {
            RuleForEach(x => x.Items).SetValidator(new AddEditProductCommandValidator(localizer));
        }
    }
    

    public class AddEditProductCommandValidator : AbstractValidator<ProductDto>
    {
        public AddEditProductCommandValidator(IStringLocalizer<AddEditProductCommandValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);
            RuleFor(request => request.Barcode)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Barcode is required!"]);
            RuleFor(request => request.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);
            RuleFor(request => request.Brand)
                .NotEmpty().Must(x => !string.IsNullOrWhiteSpace(x?.Name)).WithMessage(x => localizer["Brand is required!"]);
            RuleFor(request => request.Rate)
                .GreaterThan(0).WithMessage(x => localizer["Rate must be greater than 0"]);
        }
    }
}