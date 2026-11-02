using Coworkee.Contracts.Services;

namespace MyApp.Contracts.Catalog;

/// <summary>
/// An application service: GetAsync is GET /api/v1/catalog-statistics by convention, the Blazor client gets a generated proxy.
/// The attribute names the route, the convention would pluralize the last word.
/// </summary>
[ApplicationService(Route = "/api/v1/catalog-statistics", Tag = "Catalog statistics")]
public interface ICatalogStatisticsAppService : IApplicationService
{
    Task<CatalogStatisticsDto> GetAsync(CancellationToken cancellationToken = default);
}

public sealed record CatalogStatisticsDto(int Brands, int Products, decimal AverageRate);
