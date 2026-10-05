using Coworkee.Client.Blazor.Api;
using MyApp.Contracts;
using MyApp.Contracts.Documents;

namespace MyApp.Web.Client.Api;

internal sealed class DocumentsApi(HttpClient http) : ApiClientBase(http), IDocumentsApi
{
    private const string Types = "api/v1/document-types";
    private const string Documents = "api/v1/documents";

    public Task SaveDocumentTypeAsync(Guid? id, AddEditDocumentTypeRequest request, CancellationToken cancellationToken = default) =>
        id is { } existing ? SendAsync(HttpMethod.Put, $"{Types}/{existing}", request, cancellationToken) : SendAsync(HttpMethod.Post, Types, request, cancellationToken);

    public Task DeleteDocumentTypesAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"{Types}/delete", new IdsRequest(ids), cancellationToken);

    public async Task UploadDocumentAsync(UpdateDocumentRequest request, string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent
        {
            { new StreamContent(content), "file", fileName },
            { new StringContent(request.Title), "title" },
            { new StringContent(request.Description ?? string.Empty), "description" },
            { new StringContent(request.IsPublic ? "true" : "false"), "isPublic" },
        };
        if (request.DocumentTypeId is { } typeId)
        {
            form.Add(new StringContent(typeId.ToString()), "documentTypeId");
        }

        using var response = await SendContentAsync(HttpMethod.Post, Documents, form, cancellationToken);
    }

    public Task UpdateDocumentAsync(Guid id, UpdateDocumentRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Put, $"{Documents}/{id}", request, cancellationToken);

    public Task DeleteDocumentsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"{Documents}/delete", new IdsRequest(ids), cancellationToken);
}
