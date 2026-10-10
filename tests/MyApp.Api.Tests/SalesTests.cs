using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Coworkee.Contracts.Data;
using Coworkee.Contracts.Identity;
using Coworkee.Core.Security;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Contracts;
using MyApp.Contracts.Sales;
using MyApp.Infrastructure;
using MyApp.Sales.Domain;

namespace MyApp.Api.Tests;

public sealed class SalesTests(ApiFixture api) : IAsyncLifetime
{
    private SetupResultDto _setup = null!;
    private Guid _customer;

    public async ValueTask InitializeAsync()
    {
        _setup = await api.SetupAsync();
        await using var scope = api.Factory.Services.CreateAsyncScope();
        using var actor = CurrentUserScope.Begin(new ImpersonatedUser(null, _setup.TenantId));
        var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
        var customer = new Customer { Name = "Nordwind Handel GmbH", City = "Hamburg", TenantId = _setup.TenantId };
        db.Add(customer);
        await db.SaveChangesAsync(Ct);
        _customer = customer.Id;
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private HttpClient Admin => api.As(_setup.AdminUserId, _setup.TenantId);

    private AddEditSalesOrderRequest Order => new() { CustomerId = _customer, Channel = SalesChannel.Phone, Region = "Nord", Items = 3, Total = 59.70m };

    [Fact]
    public async Task The_application_service_writes_orders_that_the_odata_set_serves_as_a_client_entity()
    {
        var created = await (await Admin.PostAsJsonAsync("/api/v1/sales-orders", Order, Ct)).Content.ReadFromJsonAsync<SalesOrderDto>(Ct);
        created!.Number.ShouldBe("SO-0000001");
        created.Customer!.Name.ShouldBe("Nordwind Handel GmbH");
        (await Admin.PostAsJsonAsync("/api/v1/sales-orders", new AddEditSalesOrderRequest { CustomerId = _customer }, Ct)).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await Admin.PostAsJsonAsync("/api/v1/sales-orders", new AddEditSalesOrderRequest { CustomerId = Guid.NewGuid(), Region = "Nord", Items = 1 }, Ct))
            .StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var update = Order;
        update.Status = SalesOrderStatus.Confirmed;
        (await Admin.PutAsJsonAsync($"/api/v1/sales-orders/{created.Id}", update, Ct)).EnsureSuccessStatusCode();
        (await Admin.PostAsync($"/api/v1/sales-orders/{created.Id}/cancel", null, Ct)).StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await Admin.GetFromJsonAsync<SalesOrderDto>($"/api/v1/sales-orders/{created.Id}", Ct))!.Status.ShouldBe(SalesOrderStatus.Cancelled);

        var rows = (await Admin.GetFromJsonAsync<JsonElement>("/odata/SalesOrders?$expand=Customer&$filter=Status eq 'Cancelled'", Ct)).GetProperty("value");
        rows.GetArrayLength().ShouldBe(1);
        rows[0].GetProperty("Customer").GetProperty("Name").GetString().ShouldBe("Nordwind Handel GmbH");
        var set = (await Admin.GetFromJsonAsync<ClientEntityManifest>("/api/v1/client-entities", Ct))!.Sets.Single(s => s.EntitySet == "SalesOrders");
        set.MaxRows.ShouldBe(250_000, "the service raises the limit of the attribute's defaults");

        (await Admin.PostAsJsonAsync("/api/v1/sales-orders/delete", new IdsRequest([created.Id]), Ct)).StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await Admin.GetAsync($"/api/v1/sales-orders/{created.Id}", Ct)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Writing_needs_the_order_permissions()
    {
        var user = (await (await Admin.PostAsJsonAsync("/api/v1/identity/users", new CreateUserRequest("viewer@sales.test", "Passw0rd!x", null, null), Ct)).Content.ReadFromJsonAsync<UserDto>(Ct))!;
        var role = (await (await Admin.PostAsJsonAsync("/api/v1/identity/roles", new RoleRequest("Sales viewer", null), Ct)).Content.ReadFromJsonAsync<Guid>(Ct))!;
        (await Admin.PutAsJsonAsync($"/api/v1/identity/permissions/grants/Role/{role}", new NameListRequest([SalesPermissions.Orders.View]), Ct)).EnsureSuccessStatusCode();
        (await Admin.PutAsJsonAsync($"/api/v1/identity/users/{user.Id}/roles", new IdListRequest([role]), Ct)).EnsureSuccessStatusCode();
        var viewer = api.As(user.Id, _setup.TenantId);

        (await viewer.GetAsync("/odata/SalesOrders", Ct)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await viewer.PostAsJsonAsync("/api/v1/sales-orders", Order, Ct)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }
}
