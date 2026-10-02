using Coworkee.Client.Blazor;
using Coworkee.Client.Blazor.Api;
using Coworkee.Contracts;
using MyApp.Contracts.Catalog;
using MyApp.Contracts.Documents;

namespace MyApp.Web.Client;

public interface IMyAppApi
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BrandDto>> GetBrandsAsync(CancellationToken cancellationToken = default);

    Task SaveBrandAsync(Guid? id, BrandRequest request, CancellationToken cancellationToken = default);

    Task DeleteBrandAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<ProductDto>> GetProductsAsync(PageRequest page, Guid? brandId = null, CancellationToken cancellationToken = default);

    Task SaveProductAsync(Guid? id, ProductRequest request, CancellationToken cancellationToken = default);

    Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DocumentTypeDto>> GetDocumentTypesAsync(CancellationToken cancellationToken = default);

    Task SaveDocumentTypeAsync(Guid? id, DocumentTypeRequest request, CancellationToken cancellationToken = default);

    Task DeleteDocumentTypeAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<DocumentDto>> GetDocumentsAsync(PageRequest page, CancellationToken cancellationToken = default);

    Task UploadDocumentAsync(DocumentRequest request, string fileName, Stream content, CancellationToken cancellationToken = default);

    Task UpdateDocumentAsync(Guid id, DocumentRequest request, CancellationToken cancellationToken = default);

    Task DeleteDocumentAsync(Guid id, CancellationToken cancellationToken = default);
}

public static class DocumentUrls
{
    public static string Content(Guid id, bool download = false) => $"api/v1/documents/{id}/content" + (download ? "?download=true" : string.Empty);
}

internal sealed class MyAppApi(HttpClient http) : ApiClientBase(http), IMyAppApi
{
    public Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default) => GetAsync<DashboardDto>("api/v1/dashboard", cancellationToken);

    public async Task<IReadOnlyList<BrandDto>> GetBrandsAsync(CancellationToken cancellationToken = default) => await GetAsync<BrandDto[]>("api/v1/brands", cancellationToken);

    public Task SaveBrandAsync(Guid? id, BrandRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(id is null ? HttpMethod.Post : HttpMethod.Put, "api/v1/brands" + (id is { } i ? $"/{i}" : string.Empty), request, cancellationToken);

    public Task DeleteBrandAsync(Guid id, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Delete, $"api/v1/brands/{id}", null, cancellationToken);

    public Task<PagedResult<ProductDto>> GetProductsAsync(PageRequest page, Guid? brandId = null, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<ProductDto>>(
            $"api/v1/products?page={page.Page}&pageSize={page.PageSize}&search={Uri.EscapeDataString(page.Search ?? string.Empty)}" + (brandId is { } b ? $"&brandId={b}" : string.Empty),
            cancellationToken);

    public Task SaveProductAsync(Guid? id, ProductRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(id is null ? HttpMethod.Post : HttpMethod.Put, "api/v1/products" + (id is { } i ? $"/{i}" : string.Empty), request, cancellationToken);

    public Task DeleteProductAsync(Guid id, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Delete, $"api/v1/products/{id}", null, cancellationToken);

    public async Task<IReadOnlyList<DocumentTypeDto>> GetDocumentTypesAsync(CancellationToken cancellationToken = default) => await GetAsync<DocumentTypeDto[]>("api/v1/document-types", cancellationToken);

    public Task SaveDocumentTypeAsync(Guid? id, DocumentTypeRequest request, CancellationToken cancellationToken = default) =>
        SendAsync(id is null ? HttpMethod.Post : HttpMethod.Put, "api/v1/document-types" + (id is { } i ? $"/{i}" : string.Empty), request, cancellationToken);

    public Task DeleteDocumentTypeAsync(Guid id, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Delete, $"api/v1/document-types/{id}", null, cancellationToken);

    public Task<PagedResult<DocumentDto>> GetDocumentsAsync(PageRequest page, CancellationToken cancellationToken = default) =>
        GetAsync<PagedResult<DocumentDto>>($"api/v1/documents?page={page.Page}&pageSize={page.PageSize}&search={Uri.EscapeDataString(page.Search ?? string.Empty)}", cancellationToken);

    public async Task UploadDocumentAsync(DocumentRequest request, string fileName, Stream content, CancellationToken cancellationToken = default)
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

        using var response = await SendContentAsync(HttpMethod.Post, "api/v1/documents", form, cancellationToken);
    }

    public Task UpdateDocumentAsync(Guid id, DocumentRequest request, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Put, $"api/v1/documents/{id}", request, cancellationToken);

    public Task DeleteDocumentAsync(Guid id, CancellationToken cancellationToken = default) => SendAsync(HttpMethod.Delete, $"api/v1/documents/{id}", null, cancellationToken);
}

internal sealed class MyAppNavigation : INavigationContributor
{
    public IEnumerable<CoworkeeNavItem> Items =>
    [
        new("Dashboard", "/", MudBlazor.Icons.Material.Outlined.Dashboard, CatalogPermissions.View),
        new("Brands", "/catalog/brands", MudBlazor.Icons.Material.Outlined.Sell, CatalogPermissions.View),
        new("Products", "/catalog/products", MudBlazor.Icons.Material.Outlined.Inventory2, CatalogPermissions.View),
        new("Documents", "/documents", MudBlazor.Icons.Material.Outlined.Description, DocumentPermissions.View),
        new("Document types", "/documents/types", MudBlazor.Icons.Material.Outlined.Category, DocumentPermissions.Manage),
    ];
}
