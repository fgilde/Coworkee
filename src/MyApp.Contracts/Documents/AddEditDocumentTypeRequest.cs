namespace MyApp.Contracts.Documents;

public sealed class AddEditDocumentTypeRequest
{
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}
