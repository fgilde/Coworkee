using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Features.DocumentTypes.Commands.AddEdit;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Validators.Features.DocumentTypes.Commands.AddEdit
{
    public class AddEditDocumentTypesCommandValidator : AbstractValidator<AddEditDocumentTypesCommand>
    {
        public AddEditDocumentTypesCommandValidator(IStringLocalizer<AddEditDocumentTypeCommandValidator> localizer)
        {
            RuleForEach(x => x.Items).SetValidator(new AddEditDocumentTypeCommandValidator(localizer));
        }
    }
    public class AddEditDocumentTypeCommandValidator : AbstractValidator<DocumentTypeDto>
    {
        public AddEditDocumentTypeCommandValidator(IStringLocalizer<AddEditDocumentTypeCommandValidator> localizer)
        {
            RuleFor(request => request.Name)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Name is required!"]);
            RuleFor(request => request.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);
        }
    }
}