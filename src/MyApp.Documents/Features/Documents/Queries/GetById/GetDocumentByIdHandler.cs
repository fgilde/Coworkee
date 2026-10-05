using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Contracts.Documents;
using MyApp.Documents.Domain;
using MyApp.Documents.Visibility;

namespace MyApp.Documents.Features.Documents.Queries.GetById;

internal sealed class GetDocumentByIdHandler(CoworkeeDbContext db, DocumentVisibility visibility) : IHandler<GetDocumentByIdQuery, Result<DocumentDto>>
{
    public async Task<Result<DocumentDto>> HandleAsync(GetDocumentByIdQuery query, CancellationToken cancellationToken)
    {
        var visible = await visibility.VisibleAsync(db.Set<Document>().AsNoTracking().Include(d => d.DocumentType), cancellationToken);
        return await visible.SingleOrDefaultAsync(d => d.Id == query.Id, cancellationToken) is { } document ? document.ToDto() : DocumentErrors.NotFound;
    }
}
