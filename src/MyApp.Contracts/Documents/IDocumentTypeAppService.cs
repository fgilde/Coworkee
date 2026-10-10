using Coworkee.Contracts.Services;

namespace MyApp.Contracts.Documents;

/// <summary>Served under /api/v1/document-types by convention; the Blazor client gets a generated proxy.</summary>
public interface IDocumentTypeAppService : IApplicationService
{
    Task<DocumentTypeDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<DocumentTypeDto> CreateAsync(AddEditDocumentTypeRequest input, CancellationToken cancellationToken = default);

    Task<DocumentTypeDto> UpdateAsync(Guid id, AddEditDocumentTypeRequest input, CancellationToken cancellationToken = default);

    [ServiceOperation(Method = "POST", Route = "delete")]
    Task DeleteAsync(IdsRequest request, CancellationToken cancellationToken = default);
}
