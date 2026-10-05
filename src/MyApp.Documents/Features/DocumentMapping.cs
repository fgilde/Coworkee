using MyApp.Contracts.Documents;
using MyApp.Documents.Domain;

namespace MyApp.Documents.Features;

internal static class DocumentMapping
{
    public static DocumentTypeDto ToDto(this DocumentType type) => new(type.Id, type.Name, type.Description);

    public static DocumentDto ToDto(this Document document) => new(
        document.Id, document.Title, document.Description, document.IsPublic, document.DocumentTypeId, document.DocumentType?.ToDto(),
        document.FileName, document.MimeType, document.Size, document.OwnerId, document.CreatedAt);
}
