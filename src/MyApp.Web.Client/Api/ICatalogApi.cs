using MyApp.Contracts.Catalog;

namespace MyApp.Web.Client.Api;

public interface ICatalogApi
{
    Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);

    Task SaveBrandAsync(Guid? id, AddEditBrandRequest request, CancellationToken cancellationToken = default);

    Task DeleteBrandsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default);

    Task SaveProductAsync(Guid? id, AddEditProductRequest request, CancellationToken cancellationToken = default);

    Task DeleteProductsAsync(IReadOnlyList<Guid> ids, CancellationToken cancellationToken = default);
}
