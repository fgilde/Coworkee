namespace MyApp.Contracts.Catalog;

public sealed record DashboardDto(int Brands, int Products, int Documents, int DocumentTypes, int? Users, int? Roles, IReadOnlyList<MonthCountDto> ProductsPerMonth);
