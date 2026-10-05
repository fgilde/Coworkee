using FluentValidation;

namespace MyApp.Documents.Features.Documents.Commands.Update;

internal sealed class UpdateDocumentValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentValidator()
    {
        RuleFor(c => c.Document.Title).NotEmpty().MaximumLength(300);
        RuleFor(c => c.Document.Description).MaximumLength(4000);
    }
}
