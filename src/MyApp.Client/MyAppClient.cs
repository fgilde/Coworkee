using System.Globalization;
using System.Net.Http.Json;
using Coworkee.Client;
using MyApp.Contracts;
using MyApp.Contracts.Catalog;
using MyApp.Contracts.Documents;

namespace MyApp.Client;

/// <summary>
/// Typed client for the MyApp API. Give it an <see cref="HttpClient"/> whose base address is the API and whose requests carry
/// a bearer token, for example through <see cref="BearerTokenHandler"/>. Lists come from OData with <see cref="CoworkeeApiClient.QueryAsync{T}"/>.
/// </summary>
public sealed class MyAppClient(HttpClient http) : CoworkeeApiClient(http)
{
    public Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default) => GetAsync<DashboardDto>("api/v1/dashboard", cancellationToken);

    public Task<ODataResult<BrandDto>> GetBrandsAsync(string? filter = null, int top = 100, int skip = 0, CancellationToken cancellationToken = default) =>
        QueryAsync<BrandDto>("Brands", filter, "Name", top, skip, cancellationToken: cancellationToken);

    public Task<BrandDto> GetBrandAsync(Guid id, CancellationToken cancellationToken = default) => GetAsync<BrandDto>($"api/v1/brands/{id}", cancellationToken);

    public Task<BrandDto> SaveBrandAsync(Guid? id, AddEditBrandRequest brand, CancellationToken cancellationToken = default) =>
        id is { } existing
            ? SendAsync<BrandDto>(HttpMethod.Put, $"api/v1/brands/{existing}", brand, cancellationToken)
            : SendAsync<BrandDto>(HttpMethod.Post, "api/v1/brands", brand, cancellationToken);

    public Task DeleteBrandsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, "api/v1/brands/delete", new IdsRequest(ids), cancellationToken);

    public Task<ODataResult<ProductDto>> GetProductsAsync(string? filter = null, int top = 100, int skip = 0, CancellationToken cancellationToken = default) =>
        QueryAsync<ProductDto>("Products", filter, "Name", top, skip, "Brand", cancellationToken);

    public Task<ProductDto> GetProductAsync(Guid id, CancellationToken cancellationToken = default) => GetAsync<ProductDto>($"api/v1/products/{id}", cancellationToken);

    public Task<ProductDto> SaveProductAsync(Guid? id, AddEditProductRequest product, CancellationToken cancellationToken = default) =>
        id is { } existing
            ? SendAsync<ProductDto>(HttpMethod.Put, $"api/v1/products/{existing}", product, cancellationToken)
            : SendAsync<ProductDto>(HttpMethod.Post, "api/v1/products", product, cancellationToken);

    public Task DeleteProductsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, "api/v1/products/delete", new IdsRequest(ids), cancellationToken);

    public Task<ODataResult<DocumentTypeDto>> GetDocumentTypesAsync(CancellationToken cancellationToken = default) =>
        QueryAsync<DocumentTypeDto>("DocumentTypes", orderBy: "Name", top: 1000, cancellationToken: cancellationToken);

    public Task<DocumentTypeDto> SaveDocumentTypeAsync(Guid? id, AddEditDocumentTypeRequest type, CancellationToken cancellationToken = default) =>
        id is { } existing
            ? SendAsync<DocumentTypeDto>(HttpMethod.Put, $"api/v1/document-types/{existing}", type, cancellationToken)
            : SendAsync<DocumentTypeDto>(HttpMethod.Post, "api/v1/document-types", type, cancellationToken);

    public Task DeleteDocumentTypesAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, "api/v1/document-types/delete", new IdsRequest(ids), cancellationToken);

    public Task<ODataResult<DocumentDto>> GetDocumentsAsync(string? filter = null, int top = 100, int skip = 0, CancellationToken cancellationToken = default) =>
        QueryAsync<DocumentDto>("Documents", filter, "CreatedAt desc", top, skip, "DocumentType", cancellationToken);

    public Task<DocumentDto> GetDocumentAsync(Guid id, CancellationToken cancellationToken = default) => GetAsync<DocumentDto>($"api/v1/documents/{id}", cancellationToken);

    public async Task<DocumentDto> UploadDocumentAsync(UpdateDocumentRequest document, string fileName, Stream content, CancellationToken cancellationToken = default)
    {
        using var form = new MultipartFormDataContent
        {
            { new StreamContent(content), "file", fileName },
            { new StringContent(document.Title), "title" },
            { new StringContent(document.IsPublic.ToString(CultureInfo.InvariantCulture)), "isPublic" },
        };
        if (document.Description is { } description)
        {
            form.Add(new StringContent(description), "description");
        }

        if (document.DocumentTypeId is { } typeId)
        {
            form.Add(new StringContent(typeId.ToString()), "documentTypeId");
        }

        using var response = await SendContentAsync(HttpMethod.Post, "api/v1/documents", form, cancellationToken);
        return (await response.Content.ReadFromJsonAsync<DocumentDto>(Json, cancellationToken))!;
    }

    public Task<DocumentDto> UpdateDocumentAsync(Guid id, UpdateDocumentRequest document, CancellationToken cancellationToken = default) =>
        SendAsync<DocumentDto>(HttpMethod.Put, $"api/v1/documents/{id}", document, cancellationToken);

    public Task DeleteDocumentsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, "api/v1/documents/delete", new IdsRequest(ids), cancellationToken);

    /// <summary>The file of the document; dispose the stream when done.</summary>
    public async Task<Stream> OpenDocumentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response = await SendContentAsync(HttpMethod.Get, $"api/v1/documents/{id}/content?download=true", null, cancellationToken);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
