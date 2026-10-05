using Coworkee.Client.Blazor.Api;
using MyApp.Contracts;
using MyApp.Contracts.Catalog;

namespace MyApp.Web.Client.Api;

internal sealed class CatalogApi(HttpClient http) : ApiClientBase(http), ICatalogApi
{
    private const string Brands = "api/v1/brands";
    private const string Products = "api/v1/products";

    public Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default) => GetAsync<DashboardDto>("api/v1/dashboard", cancellationToken);

    public Task SaveBrandAsync(Guid? id, AddEditBrandRequest request, CancellationToken cancellationToken = default) =>
        SaveAsync(Brands, id, request, cancellationToken);

    public Task DeleteBrandsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"{Brands}/delete", new IdsRequest(ids), cancellationToken);

    public Task SaveProductAsync(Guid? id, AddEditProductRequest request, CancellationToken cancellationToken = default) =>
        SaveAsync(Products, id, request, cancellationToken);

    public Task DeleteProductsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default) =>
        SendAsync(HttpMethod.Post, $"{Products}/delete", new IdsRequest(ids), cancellationToken);

    private Task SaveAsync(string resource, Guid? id, object request, CancellationToken cancellationToken) =>
        id is { } existing ? SendAsync(HttpMethod.Put, $"{resource}/{existing}", request, cancellationToken) : SendAsync(HttpMethod.Post, resource, request, cancellationToken);
}
