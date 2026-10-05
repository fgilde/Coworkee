using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.Dashboard;
using MyApp.Documents.Domain;

namespace MyApp.Documents.Features.Dashboard;

internal sealed class DocumentDashboardCounts(CoworkeeDbContext db) : IDashboardCounts
{
    public async Task<(int Documents, int DocumentTypes)> DocumentsAsync(CancellationToken cancellationToken) =>
        (await db.Set<Document>().CountAsync(cancellationToken), await db.Set<DocumentType>().CountAsync(cancellationToken));
}
