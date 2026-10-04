namespace MyApp.Contracts.Documents;

public static class DocumentPermissions
{
    public const string GroupName = "Documents";
    public const string View = "Documents.View";
    public const string Upload = "Documents.Upload";

    /// <summary>Edit and delete every document and manage document types; without it users manage only their own.</summary>
    public const string Manage = "Documents.Manage";
}

public sealed record DocumentTypeDto(Guid Id, string Name, string? Description);

public sealed record DocumentTypeRequest(string Name, string? Description = null);

/// <summary>A stored file; public documents are visible to everyone who may view documents, private ones to their owner and managers.</summary>
public sealed record DocumentDto(
    Guid Id, string Title, string? Description, bool IsPublic, Guid? DocumentTypeId, string? DocumentTypeName, string FileName, string MimeType, long Size,
    Guid? OwnerId, DateTimeOffset CreatedAt, bool CanEdit);

public sealed record DocumentRequest(string Title, string? Description = null, bool IsPublic = false, Guid? DocumentTypeId = null);

public static class DocumentInlineTypes
{
    /// <summary>Shown in the browser; everything else is only downloaded (no HTML or SVG in the app's origin).</summary>
    public static readonly IReadOnlySet<string> All = new HashSet<string>
    {
        "application/pdf", "image/jpeg", "image/png", "image/gif", "image/webp", "video/mp4", "video/webm", "audio/mpeg", "audio/wav", "audio/ogg", "text/plain", "text/csv",
    };
}
