using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Core.Security;
using Coworkee.Infrastructure.Persistence;
using Coworkee.Storage;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using MyApp.Contracts.Documents;
using MyApp.Documents.Domain;

namespace MyApp.Documents.Features.Documents.Commands.Upload;

internal sealed class UploadDocumentHandler(CoworkeeDbContext db, ICurrentUser currentUser, IBlobStorage storage, TimeProvider clock)
    : IHandler<UploadDocumentCommand, Result<DocumentDto>>
{
    private static readonly FileExtensionContentTypeProvider ContentTypes = new();

    public async Task<Result<DocumentDto>> HandleAsync(UploadDocumentCommand command, CancellationToken cancellationToken)
    {
        if (currentUser.TenantId is not { } tenantId)
        {
            return DocumentErrors.NoTenant;
        }

        var type = command.Document.DocumentTypeId is { } typeId ? await db.Set<DocumentType>().SingleOrDefaultAsync(t => t.Id == typeId, cancellationToken) : null;
        if (command.Document.DocumentTypeId is not null && type is null)
        {
            return DocumentErrors.UnknownType;
        }

        var fileName = UploadFileName.Clean(command.FileName);
        var mimeType = ContentTypes.TryGetContentType(fileName, out var contentType) ? contentType : "application/octet-stream";
        var key = BlobKeys.New(tenantId, clock);
        await storage.PutAsync(key, command.Content, mimeType, cancellationToken);
        var document = new Document
        {
            Title = command.Document.Title.Trim(),
            Description = string.IsNullOrWhiteSpace(command.Document.Description) ? null : command.Document.Description.Trim(),
            IsPublic = command.Document.IsPublic,
            DocumentTypeId = type?.Id,
            DocumentType = type,
            FileName = fileName,
            MimeType = mimeType,
            Size = command.Size,
            BlobKey = key,
            OwnerId = currentUser.UserId,
        };
        db.Add(document);
        return document.ToDto();
    }
}
