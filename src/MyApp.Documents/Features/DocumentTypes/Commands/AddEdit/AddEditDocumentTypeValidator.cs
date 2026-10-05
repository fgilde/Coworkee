using FluentValidation;

namespace MyApp.Documents.Features.DocumentTypes.Commands.AddEdit;

internal sealed class AddEditDocumentTypeValidator : AbstractValidator<AddEditDocumentTypeCommand>
{
    public AddEditDocumentTypeValidator()
    {
        RuleFor(c => c.Type.Name).NotEmpty().MaximumLength(200);
        RuleFor(c => c.Type.Description).MaximumLength(2000);
    }
}
