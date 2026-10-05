using FluentValidation;

namespace MyApp.Documents.Features.Documents.Commands.Upload;

internal sealed class UploadDocumentValidator : AbstractValidator<UploadDocumentCommand>
{
    private static readonly HashSet<string> Blocked =
    [
        ".exe", ".dll", ".bat", ".cmd", ".com", ".msi", ".msix", ".appx", ".scr", ".ps1", ".psm1", ".vbs", ".vbe", ".js", ".mjs", ".jse", ".wsf", ".wsh", ".jar", ".sh",
        ".hta", ".lnk", ".url", ".reg", ".cpl", ".pif", ".application", ".iso", ".inf", ".msc",
    ];

    public UploadDocumentValidator()
    {
        RuleFor(c => c.Document.Title).NotEmpty().MaximumLength(300);
        RuleFor(c => c.Document.Description).MaximumLength(4000);
        RuleFor(c => UploadFileName.Clean(c.FileName)).NotEmpty().MaximumLength(255).Must(name => !name.Any(char.IsControl)).OverridePropertyName("File");
        RuleFor(c => Path.GetExtension(UploadFileName.Clean(c.FileName)).ToLowerInvariant())
            .Must(extension => !Blocked.Contains(extension)).WithMessage("Files of this type cannot be uploaded.").OverridePropertyName("File");
        RuleFor(c => c.Size).InclusiveBetween(1, MyAppDocumentsModule.MaxSize).OverridePropertyName("File");
    }
}
