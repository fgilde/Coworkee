namespace MyApp.Contracts.Documents;

public sealed class UpdateDocumentRequest
{
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsPublic { get; set; }

    public Guid? DocumentTypeId { get; set; }
}
