using System.Net;
using System.Net.Http.Json;
using Coworkee.Contracts.Identity;
using Coworkee.Contracts.Reporting;
using Coworkee.Core.Security;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Contracts.Sales;
using MyApp.Infrastructure;
using MyApp.Sales.Domain;

namespace MyApp.Api.Tests;

public sealed class ReportsTests(ApiFixture api) : IAsyncLifetime
{
    private SetupResultDto _setup = null!;

    public async ValueTask InitializeAsync()
    {
        _setup = await api.SetupAsync();
        await using var scope = api.Factory.Services.CreateAsyncScope();
        using var actor = CurrentUserScope.Begin(new ImpersonatedUser(null, _setup.TenantId));
        var db = scope.ServiceProvider.GetRequiredService<MyAppDbContext>();
        var customer = new Customer { Name = "Hanse Logistik KG", City = "Bremen", TenantId = _setup.TenantId };
        db.Add(customer);
        var day = new DateTimeOffset(2026, 3, 2, 10, 0, 0, TimeSpan.Zero);
        db.AddRange(Enumerable.Range(0, 12).Select(i => new SalesOrder
        {
            Number = $"SO-{i:0000000}", CustomerId = customer.Id, Status = (SalesOrderStatus)(i % 3), Channel = SalesChannel.Online,
            Region = i < 8 ? "Nord" : "Süd", OrderDate = day.AddDays(i * 10), Items = 1, Total = 100, TenantId = _setup.TenantId,
        }));
        await db.SaveChangesAsync(Ct);
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private HttpClient Admin => api.As(_setup.AdminUserId, _setup.TenantId);

    [Fact]
    public async Task Sales_orders_aggregate_on_the_server_by_region_and_month_with_customer_names()
    {
        var byRegion = await QueryAsync(new ReportQueryRequest("SalesOrders", GroupBy: new ReportGroupBy("Region"), Aggregates: [new(), new(AggregateFunction.Sum, "Total")]));
        byRegion.Rows.Select(r => (r.Key!.Value, r.Values[0], r.Values[1])).ShouldBe([("Nord", 8d, 800d), ("Süd", 4d, 400d)]);

        var byMonth = await QueryAsync(new ReportQueryRequest("SalesOrders", "Region eq 'Nord'", new ReportGroupBy("OrderDate", DateBucket.Month), new ReportGroupBy("Customer/Name")));
        byMonth.Rows.Sum(r => r.Values[0]).ShouldBe(8);
        byMonth.Rows.ShouldAllBe(r => r.Series!.Value == "Hanse Logistik KG");
        (await QueryAsync(new ReportQueryRequest("SalesOrders", byMonth.Rows[0].Key!.Filter))).Rows.Single().Values[0].ShouldBe(byMonth.Rows[0].Values[0]);
    }

    [Fact]
    public async Task A_report_over_sales_orders_is_shared_with_a_role_and_its_data_needs_the_order_permission()
    {
        var definition = new ReportDefinition
        {
            Widgets = [new ReportWidget { Id = "orders", Type = WidgetType.Kpi, Query = new WidgetQuery { EntitySet = "SalesOrders", DateField = "OrderDate" } }],
        };
        var role = await (await Admin.PostAsJsonAsync("/api/v1/identity/roles", new RoleRequest("Report readers", null), Ct)).Content.ReadFromJsonAsync<Guid>(Ct);
        (await Admin.PutAsJsonAsync($"/api/v1/identity/permissions/grants/Role/{role}", new NameListRequest([ReportPermissions.View]), Ct)).EnsureSuccessStatusCode();
        var user = (await (await Admin.PostAsJsonAsync("/api/v1/identity/users", new CreateUserRequest("reader@sales.test", "Passw0rd!x", null, null), Ct)).Content.ReadFromJsonAsync<UserDto>(Ct))!;
        (await Admin.PutAsJsonAsync($"/api/v1/identity/users/{user.Id}/roles", new IdListRequest([role]), Ct)).EnsureSuccessStatusCode();
        var reader = api.As(user.Id, _setup.TenantId);

        (await Admin.PostAsJsonAsync("/api/v1/reports", new SaveReportRequest("Orders", null, null, ReportVisibility.Roles, [role], definition), Ct)).EnsureSuccessStatusCode();

        (await reader.GetFromJsonAsync<List<ReportSummaryDto>>("/api/v1/reports", Ct))!.ShouldHaveSingleItem().EntitySets.ShouldBe(["SalesOrders"]);
        (await reader.PostAsJsonAsync("/api/v1/reports/query", new ReportQueryRequest("SalesOrders"), Ct)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await reader.GetFromJsonAsync<List<ReportSourceDto>>("/api/v1/reports/sources", Ct))!.ShouldNotContain(s => s.EntitySet == "SalesOrders");
    }

    private async Task<ReportQueryResult> QueryAsync(ReportQueryRequest request)
    {
        var response = await Admin.PostAsJsonAsync("/api/v1/reports/query", request, Ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ReportQueryResult>(Ct))!;
    }
}
