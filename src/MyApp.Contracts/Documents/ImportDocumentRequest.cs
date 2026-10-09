namespace MyApp.Contracts.Documents;

/// <summary>Copies a stored file of the Files module into a new document; the file stays where it is.</summary>
public sealed record ImportDocumentRequest(Guid FileId, UpdateDocumentRequest Document);
