using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Documents.Domain;

namespace MyApp.Documents.Features.DocumentTypes.Commands.Delete;

internal sealed class DeleteDocumentTypesHandler(CoworkeeDbContext db) : IHandler<DeleteDocumentTypesCommand, Result>
{
    public async Task<Result> HandleAsync(DeleteDocumentTypesCommand command, CancellationToken cancellationToken)
    {
        var types = await db.Set<DocumentType>().Where(t => command.Ids.Contains(t.Id)).ToListAsync(cancellationToken);
        if (types.Count != command.Ids.Distinct().Count())
        {
            return DocumentErrors.TypeNotFound;
        }

        db.RemoveRange(types);
        return Result.Success();
    }
}
