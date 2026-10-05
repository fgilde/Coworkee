using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Contracts.Documents;
using MyApp.Documents.Domain;
using MyApp.Documents.Visibility;

namespace MyApp.Documents.Features.Documents.Commands.Update;

internal sealed class UpdateDocumentHandler(CoworkeeDbContext db, DocumentVisibility visibility) : IHandler<UpdateDocumentCommand, Result<DocumentDto>>
{
    public async Task<Result<DocumentDto>> HandleAsync(UpdateDocumentCommand command, CancellationToken cancellationToken)
    {
        var editable = await visibility.EditableAsync(db.Set<Document>().Include(d => d.DocumentType), cancellationToken);
        if (await editable.SingleOrDefaultAsync(d => d.Id == command.Id, cancellationToken) is not { } document)
        {
            return DocumentErrors.NotFound;
        }

        var type = command.Document.DocumentTypeId is { } typeId ? await db.Set<DocumentType>().SingleOrDefaultAsync(t => t.Id == typeId, cancellationToken) : null;
        if (command.Document.DocumentTypeId is not null && type is null)
        {
            return DocumentErrors.UnknownType;
        }

        document.Title = command.Document.Title.Trim();
        document.Description = string.IsNullOrWhiteSpace(command.Document.Description) ? null : command.Document.Description.Trim();
        document.IsPublic = command.Document.IsPublic;
        document.DocumentTypeId = type?.Id;
        document.DocumentType = type;
        return document.ToDto();
    }
}
