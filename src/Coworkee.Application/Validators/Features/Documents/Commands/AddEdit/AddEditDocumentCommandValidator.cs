using Coworkee.Application.Common.Models;
using Coworkee.Application.Features.Documents.Commands.AddEdit;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Validators.Features.Documents.Commands.AddEdit
{
    public class AddEditDocumentsCommandValidator : AbstractValidator<AddEditDocumentsCommand>
    {
        public AddEditDocumentsCommandValidator(IStringLocalizer<AddEditDocumentCommandValidator> localizer)
        {
            RuleForEach(x => x.Items).SetValidator(new AddEditDocumentCommandValidator(localizer));
        }
    }

    public class AddEditDocumentCommandValidator : AbstractValidator<DocumentDto>
    {
        public AddEditDocumentCommandValidator(IStringLocalizer<AddEditDocumentCommandValidator> localizer)
        {
            RuleFor(request => request.Title)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Title is required!"]);
            RuleFor(request => request.Description)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage(x => localizer["Description is required!"]);
            RuleFor(request => request)
                .Must(r => !string.IsNullOrWhiteSpace(r.URL) || r.UploadRequest != null).WithMessage(x => localizer["File is required!"]);
        }
    }
}