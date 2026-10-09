using FluentValidation;

namespace MyApp.Documents.Features.Documents.Commands.Import;

internal sealed class ImportDocumentValidator : AbstractValidator<ImportDocumentCommand>
{
    public ImportDocumentValidator()
    {
        RuleFor(c => c.Request.FileId).NotEmpty().OverridePropertyName("FileId");
        RuleFor(c => c.Request.Document).NotNull().OverridePropertyName("Document");
    }
}
