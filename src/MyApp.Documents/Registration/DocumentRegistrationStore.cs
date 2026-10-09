using Coworkee.Application.Registration;
using Coworkee.Core.Security;
using Coworkee.Infrastructure.Persistence;
using Coworkee.Storage;
using Microsoft.EntityFrameworkCore;
using MyApp.Documents.Domain;
using MyApp.Documents.Features.Documents.Commands.Upload;

namespace MyApp.Documents.Registration;

/// <summary>Registration documents become private documents of the new user with the type "Registration"; admins see them through Documents.ManageAll.</summary>
internal sealed class DocumentRegistrationStore(CoworkeeDbContext db, ICurrentUser currentUser, IBlobStorage storage, TimeProvider clock) : IRegistrationDocumentStore
{
    public const string TypeName = "Registration";

    public async Task SaveAsync(RegistrationDocument document, CancellationToken cancellationToken)
    {
        var tenantId = currentUser.TenantId ?? throw new InvalidOperationException("Registration documents need a tenant.");
        var type = db.Set<DocumentType>().Local.FirstOrDefault(t => t.Name == TypeName)
            ?? await db.Set<DocumentType>().FirstOrDefaultAsync(t => t.Name == TypeName, cancellationToken);
        if (type is null)
        {
            type = new DocumentType { Name = TypeName, Description = "Uploaded by new users when they registered." };
            db.Add(type);
        }

        var key = BlobKeys.New(tenantId, clock);
        await storage.PutAsync(key, document.Content, document.ContentType, cancellationToken);
        db.Add(new Document
        {
            Title = document.Slot.Name,
            Description = document.Slot.Description,
            IsPublic = false,
            DocumentTypeId = type.Id,
            DocumentType = type,
            OwnerId = document.UserId,
            FileName = UploadFileName.Clean(document.FileName),
            MimeType = document.ContentType.Length <= 100 ? document.ContentType : "application/octet-stream",
            Size = document.Size,
            BlobKey = key,
        });
    }
}
