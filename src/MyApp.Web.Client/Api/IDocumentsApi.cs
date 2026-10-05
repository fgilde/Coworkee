using MyApp.Contracts.Documents;

namespace MyApp.Web.Client.Api;

public interface IDocumentsApi
{
    Task SaveDocumentTypeAsync(Guid? id, AddEditDocumentTypeRequest request, CancellationToken cancellationToken = default);

    Task DeleteDocumentTypesAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default);

    Task UploadDocumentAsync(UpdateDocumentRequest request, string fileName, Stream content, CancellationToken cancellationToken = default);

    Task UpdateDocumentAsync(Guid id, UpdateDocumentRequest request, CancellationToken cancellationToken = default);

    Task DeleteDocumentsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default);
}
