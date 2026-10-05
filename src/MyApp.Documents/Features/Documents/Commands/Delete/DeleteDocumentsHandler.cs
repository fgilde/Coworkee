using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Coworkee.Storage;
using Microsoft.EntityFrameworkCore;
using MyApp.Documents.Domain;
using MyApp.Documents.Visibility;

namespace MyApp.Documents.Features.Documents.Commands.Delete;

internal sealed class DeleteDocumentsHandler(CoworkeeDbContext db, DocumentVisibility visibility, IBlobStorage storage) : IHandler<DeleteDocumentsCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteDocumentsCommand command, CancellationToken cancellationToken)
    {
        var editable = await visibility.EditableAsync(db.Set<Document>(), cancellationToken);
        var documents = await editable.Where(d => command.Ids.Contains(d.Id)).ToListAsync(cancellationToken);
        if (documents.Count != command.Ids.Distinct().Count())
        {
            return DocumentErrors.NotFound;
        }

        foreach (var document in documents)
        {
            await storage.DeleteAsync(document.BlobKey, cancellationToken);
        }

        db.RemoveRange(documents);
        return Result.Success();
    }
}
