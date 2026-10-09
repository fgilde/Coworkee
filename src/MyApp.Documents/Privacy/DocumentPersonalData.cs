using Coworkee.Application.Privacy;
using Coworkee.Infrastructure.Persistence;
using Coworkee.Storage;
using Microsoft.EntityFrameworkCore;
using MyApp.Documents.Domain;

namespace MyApp.Documents.Privacy;

/// <summary>Documents the user owns. Erasing deletes the private ones with their files; public documents belong to the organisation and stay.</summary>
internal sealed class DocumentPersonalData(CoworkeeDbContext db, IBlobStorage storage) : IPersonalDataContributor
{
    public string Section => "documents";

    public async Task<object?> ExportAsync(PersonalDataSubject subject, CancellationToken cancellationToken) =>
        await Owned(subject).AsNoTracking().OrderBy(d => d.CreatedAt)
            .Select(d => new { d.Id, d.Title, d.Description, d.FileName, d.MimeType, d.Size, d.IsPublic, d.CreatedAt })
            .ToListAsync(cancellationToken);

    public async Task EraseAsync(PersonalDataSubject subject, CancellationToken cancellationToken)
    {
        var documents = await Owned(subject).Where(d => !d.IsPublic).ToListAsync(cancellationToken);
        foreach (var document in documents)
        {
            await storage.DeleteAsync(document.BlobKey, cancellationToken);
        }

        db.RemoveRange(documents);
    }

    private IQueryable<Document> Owned(PersonalDataSubject subject) =>
        db.Set<Document>().IgnoreQueryFilters().Where(d => d.TenantId == subject.TenantId && d.OwnerId == subject.UserId);
}
