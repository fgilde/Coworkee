using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Coworkee.OData.ClientEntities;
using Microsoft.EntityFrameworkCore;
using MyApp.Documents.Domain;

namespace MyApp.Documents.Features.DocumentTypes.Commands.Delete;

internal sealed class DeleteDocumentTypesHandler(CoworkeeDbContext db, IClientEntityChangeLog changeLog) : IHandler<DeleteDocumentTypesCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteDocumentTypesCommand command, CancellationToken cancellationToken)
    {
        var types = await db.Set<DocumentType>().Where(t => command.Ids.Contains(t.Id)).ToListAsync(cancellationToken);
        if (types.Count != command.Ids.Distinct().Count())
        {
            return DocumentErrors.TypeNotFound;
        }

        // the database sets DocumentTypeId of their documents to null, which moves no ModifiedAt for the client copies
        changeLog.Touch<Document>(await db.Set<Document>().Where(d => d.DocumentTypeId != null && command.Ids.Contains(d.DocumentTypeId.Value)).Select(d => d.Id).ToListAsync(cancellationToken));
        db.RemoveRange(types);
        return Result.Success();
    }
}
