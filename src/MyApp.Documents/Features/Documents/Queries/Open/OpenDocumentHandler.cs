using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Coworkee.Storage;
using Microsoft.EntityFrameworkCore;
using MyApp.Documents.Domain;
using MyApp.Documents.Visibility;

namespace MyApp.Documents.Features.Documents.Queries.Open;

internal sealed class OpenDocumentHandler(CoworkeeDbContext db, DocumentVisibility visibility, IBlobStorage storage) : IHandler<OpenDocumentQuery, Result<DocumentContent>>
{
    public async Task<Result<DocumentContent>> HandleAsync(OpenDocumentQuery query, CancellationToken cancellationToken)
    {
        var visible = await visibility.VisibleAsync(db.Set<Document>().AsNoTracking(), cancellationToken);
        if (await visible.SingleOrDefaultAsync(d => d.Id == query.Id, cancellationToken) is not { } document
            || await storage.OpenReadAsync(document.BlobKey, cancellationToken) is not { } content)
        {
            return DocumentErrors.NotFound;
        }

        return new DocumentContent(content, document.FileName, document.MimeType);
    }
}
