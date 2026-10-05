namespace MyApp.Contracts.Documents;

public sealed record DocumentDto(
    Guid Id, string Title, string? Description, bool IsPublic, Guid? DocumentTypeId, DocumentTypeDto? DocumentType, string FileName, string MimeType, long Size,
    Guid? OwnerId, DateTimeOffset CreatedAt);
