using System.Net;
using System.Net.Http.Json;
using Coworkee.Contracts.Identity;
using Coworkee.Core.Security;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Catalog.Domain;
using MyApp.Contracts.Catalog;
using MyApp.Infrastructure;

namespace MyApp.Api.Tests;

/// <summary>The ways to give a use case a route: on the request, through a controller, as an application service.</summary>
public sealed class RoutingTests(ApiFixture api) : IAsyncLifetime
{
    private SetupResultDto _setup = null!;
    private Guid _brand;

    public async ValueTask InitializeAsync()
    {
        _setup = await api.SetupAsync();
        _brand = await SeedAsync(_setup.TenantId, "Acme", ("Saw", "4002", 10m), ("Hammer", "4001", 20m));
        await SeedAsync(Guid.NewGuid(), "Other", ("Foreign", "4001", 99m));
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private HttpClient Admin => api.As(_setup.AdminUserId, _setup.TenantId);

    [Fact]
    public async Task A_request_with_a_route_attribute_is_an_endpoint()
    {
        var products = await Admin.GetFromJsonAsync<List<ProductDto>>($"/api/v1/brands/{_brand}/products", Ct);

        products!.Select(p => p.Name).ShouldBe(["Hammer", "Saw"]);
        (await Admin.GetAsync($"/api/v1/brands/{Guid.NewGuid()}/products", Ct)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task A_controller_in_a_module_dispatches_a_request()
    {
        var product = await Admin.GetFromJsonAsync<ProductDto>("/api/v1/product-lookup/by-barcode/4001", Ct);

        product!.Name.ShouldBe("Hammer", "the product of the other tenant stays hidden");
        (await Admin.GetAsync("/api/v1/product-lookup/by-barcode/nope", Ct)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task An_application_service_counts_only_the_own_tenant() =>
        (await Admin.GetFromJsonAsync<CatalogStatisticsDto>("/api/v1/catalog-statistics", Ct)).ShouldBe(new CatalogStatisticsDto(1, 2, 15m));

    [Fact]
    public async Task All_three_ways_check_the_permission()
    {
        var nobody = await UserWithoutPermissionsAsync();

        (await nobody.GetAsync($"/api/v1/brands/{_brand}/products", Ct)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await nobody.GetAsync("/api/v1/product-lookup/by-barcode/4001", Ct)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await nobody.GetAsync("/api/v1/catalog-statistics", Ct)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task All_three_ways_need_a_signed_in_user()
    {
        var anonymous = api.Factory.CreateClient();

        (await anonymous.GetAsync($"/api/v1/brands/{_brand}/products", Ct)).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await anonymous.GetAsync("/api/v1/product-lookup/by-barcode/4001", Ct)).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        (await anonymous.GetAsync("/api/v1/catalog-statistics", Ct)).StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
    }

    private async Task<Guid> SeedAsync(Guid tenant, string brandName, params (string Name, string Barcode, decimal Rate)[] products)
    {
        await using var scope = api.Factory.Services.CreateAsyncScope();
        using var actor = CurrentUserScope.Begin(new ImpersonatedUser(null, tenant));
        var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
        var brand = new Brand { Name = brandName, TenantId = tenant };
        db.Add(brand);
        db.AddRange(products.Select(p => new Product { Name = p.Name, Barcode = p.Barcode, Rate = p.Rate, Brand = brand, TenantId = tenant }));
        await db.SaveChangesAsync(Ct);
        return brand.Id;
    }

    private async Task<HttpClient> UserWithoutPermissionsAsync()
    {
        var user = (await (await Admin.PostAsJsonAsync("/api/v1/identity/users", new CreateUserRequest("nobody@routing.test", "Passw0rd!x", null, null), Ct))
            .Content.ReadFromJsonAsync<UserDto>(Ct))!;
        // a role without permissions replaces the default roles of new users
        var role = (await (await Admin.PostAsJsonAsync("/api/v1/identity/roles", new RoleRequest("Nothing", null), Ct)).Content.ReadFromJsonAsync<Guid>(Ct))!;
        (await Admin.PutAsJsonAsync($"/api/v1/identity/users/{user.Id}/roles", new IdListRequest([role]), Ct)).EnsureSuccessStatusCode();
        return api.As(user.Id, _setup.TenantId);
    }
}
