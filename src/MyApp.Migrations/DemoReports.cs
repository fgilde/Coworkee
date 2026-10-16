using System.Text.Json;
using Coworkee.Contracts.Reporting;
using Coworkee.Core.Security;
using Coworkee.Identity.Domain;
using Coworkee.Reporting;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure;

namespace MyApp.Migrations;

/// <summary>Prebuilt reports of the administrator, once per database: sales over the 100,000 orders (the organisation's dashboard), an order desk and the catalog.</summary>
public static class DemoReports
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static async Task EnsureAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        if (await provider.GetRequiredService<ITenantDirectory>().GetSystemTenantIdAsync(cancellationToken) is not { } tenantId)
        {
            return;
        }

        using var actor = CurrentUserScope.Begin(new ImpersonatedUser(null, tenantId));
        var db = provider.GetRequiredService<MyAppDbContext>();
        if (!await db.Set<Report>().AnyAsync(cancellationToken)
            && await db.Set<User>().FirstOrDefaultAsync(u => u.Email == DemoSeed.Administrator.Email, cancellationToken) is { } owner)
        {
            await AddReportsAsync(db, tenantId, owner.Id, cancellationToken);
        }

        // ponytail: also databases seeded before; an organisation dashboard cleared by hand comes back with the next run of the migrations host
        if (!await db.Set<DashboardReport>().AnyAsync(d => d.UserId == null, cancellationToken)
            && await db.Set<Report>().FirstOrDefaultAsync(r => r.Name == SalesOverviewName, cancellationToken) is { } sales)
        {
            db.Add(new DashboardReport { TenantId = tenantId, ReportId = sales.Id });
            await db.SaveChangesAsync(cancellationToken);
        }
    }

    private const string SalesOverviewName = "Sales overview";

    private static async Task AddReportsAsync(MyAppDbContext db, Guid tenantId, Guid ownerId, CancellationToken cancellationToken)
    {
        var roles = await db.Set<Role>().Where(r => r.TenantId == tenantId).ToDictionaryAsync(r => r.Name!, r => r.Id, cancellationToken);
        List<Guid> Roles(params string[] names) => [.. names.Where(roles.ContainsKey).Select(n => roles[n])];
        db.AddRange(
            Create(tenantId, ownerId, SalesOverviewName, "Revenue, orders and customers of the last twelve months across regions and channels.",
                null, Roles(DemoSeed.SalesManager), SalesOverview()),
            Create(tenantId, ownerId, "Order desk", "Open orders to follow up, grouped and searchable.",
                null, Roles(DemoSeed.SalesManager), OrderDesk()),
            Create(tenantId, ownerId, "Catalog", "Products and brands at a glance.",
                null, Roles("Product Manager", "Brand Manager"), Catalog()));
        await db.SaveChangesAsync(cancellationToken);
    }

    private static Report Create(Guid tenantId, Guid ownerId, string name, string description, string? icon, List<Guid> roles, ReportDefinition definition) => new()
    {
        TenantId = tenantId,
        OwnerId = ownerId,
        Name = name,
        Description = description,
        Icon = icon,
        Visibility = roles.Count > 0 ? ReportVisibility.Roles : ReportVisibility.Private,
        RoleIds = roles,
        Definition = JsonSerializer.Serialize(definition, Json),
    };

    private const string Orders = "SalesOrders";
    private const string Open = "Status eq MyApp.Contracts.Sales.SalesOrderStatus'Open'";

    private static ReportWidget Kpi(string id, string title, int x, ReportAggregate value, NumberFormat format, DateBucket sparkline, string? filter = null) => new()
    {
        Id = id, Type = WidgetType.Kpi, Title = title, X = x, Y = 0, W = 3, H = 3,
        Query = new WidgetQuery { EntitySet = Orders, Aggregates = [value], DateField = "OrderDate", Filter = filter },
        Options = new WidgetOptions { Format = format, Sparkline = sparkline },
    };

    private static ReportWidget Chart(string id, string title, ChartKind kind, int x, int y, int w, int h, WidgetQuery query, NumberFormat format = NumberFormat.Number) => new()
    {
        Id = id, Type = WidgetType.Chart, Title = title, X = x, Y = y, W = w, H = h, Query = query,
        Options = new WidgetOptions { Chart = kind, Format = format },
    };

    private static ReportDefinition SalesOverview() => new()
    {
        DateRange = new ReportDateRange(DateRangePreset.Last12Months),
        Widgets =
        [
            Kpi("revenue", "Revenue", 0, new(AggregateFunction.Sum, "Total"), NumberFormat.Currency, DateBucket.Month),
            Kpi("orders", "Orders", 3, new(), NumberFormat.Integer, DateBucket.Week),
            Kpi("average", "Average order", 6, new(AggregateFunction.Average, "Total"), NumberFormat.Currency, DateBucket.None),
            Kpi("customers", "Customers", 9, new(AggregateFunction.DistinctCount, "CustomerId"), NumberFormat.Integer, DateBucket.None),
            Chart("by-month", "Revenue by month", ChartKind.Area, 0, 3, 8, 6, new WidgetQuery
            {
                EntitySet = Orders, DateField = "OrderDate", GroupBy = new ReportGroupBy("OrderDate", DateBucket.Month), Aggregates = [new(AggregateFunction.Sum, "Total", "Revenue")],
            }, NumberFormat.Currency),
            Chart("by-status", "Orders by status", ChartKind.Donut, 8, 3, 4, 6, new WidgetQuery
            {
                EntitySet = Orders, DateField = "OrderDate", GroupBy = new ReportGroupBy("Status"), Aggregates = [new()],
            }),
            Chart("region-channel", "Revenue by region and channel", ChartKind.StackedBar, 0, 9, 6, 6, new WidgetQuery
            {
                EntitySet = Orders, DateField = "OrderDate", GroupBy = new ReportGroupBy("Region"), SplitBy = new ReportGroupBy("Channel"), Aggregates = [new(AggregateFunction.Sum, "Total")],
            }, NumberFormat.Compact),
            Chart("top-customers", "Top 10 customers", ChartKind.HorizontalBar, 6, 9, 6, 6, new WidgetQuery
            {
                EntitySet = Orders, DateField = "OrderDate", GroupBy = new ReportGroupBy("Customer/Name"), Aggregates = [new(AggregateFunction.Sum, "Total", "Revenue")],
                SortBy = ReportSortBy.Value, Descending = true, Top = 10,
            }, NumberFormat.Currency),
            new ReportWidget
            {
                Id = "pivot", Type = WidgetType.Pivot, Title = "Orders by region and status", X = 0, Y = 15, W = 6, H = 6,
                Query = new WidgetQuery { EntitySet = Orders, DateField = "OrderDate", GroupBy = new ReportGroupBy("Region"), SplitBy = new ReportGroupBy("Status"), Aggregates = [new()] },
                Options = new WidgetOptions { Format = NumberFormat.Integer },
            },
            Chart("per-week", "Orders per week", ChartKind.Line, 6, 15, 6, 6, new WidgetQuery
            {
                EntitySet = Orders, DateField = "OrderDate", GroupBy = new ReportGroupBy("OrderDate", DateBucket.Week), Aggregates = [new(AggregateFunction.Count, null, "Orders")],
            }),
            new ReportWidget
            {
                Id = "latest", Type = WidgetType.Table, Title = "Latest orders", X = 0, Y = 21, W = 12, H = 7,
                Query = new WidgetQuery
                {
                    EntitySet = Orders, DateField = "OrderDate", Columns = ["Number", "Customer/Name", "Status", "Channel", "Region", "OrderDate", "Items", "Total"],
                    OrderBy = "OrderDate", Descending = true,
                },
            },
        ],
    };

    private static ReportDefinition OrderDesk() => new()
    {
        Widgets =
        [
            new ReportWidget
            {
                Id = "intro", Type = WidgetType.Markdown, Title = "How to use", X = 0, Y = 0, W = 4, H = 3,
                Options = new WidgetOptions { Text = "Open orders wait for confirmation.\n\n- Group the table by **region** or **channel**\n- Click a bar to narrow the table\n- *Open the data* jumps to the sales orders page" },
            },
            Kpi("open", "Open orders", 4, new(), NumberFormat.Integer, DateBucket.None, Open),
            new ReportWidget
            {
                Id = "open-value", Type = WidgetType.Kpi, Title = "Open order value", X = 7, Y = 0, W = 5, H = 3,
                Query = new WidgetQuery { EntitySet = Orders, Aggregates = [new(AggregateFunction.Sum, "Total")], Filter = Open },
                Options = new WidgetOptions { Format = NumberFormat.Currency, Target = 250_000 },
            },
            Chart("open-region", "Open orders by region", ChartKind.Bar, 0, 3, 12, 4, new WidgetQuery
            {
                EntitySet = Orders, Filter = Open, GroupBy = new ReportGroupBy("Region"), SplitBy = new ReportGroupBy("Channel"), Aggregates = [new()],
            }),
            new ReportWidget
            {
                Id = "desk", Type = WidgetType.DataTable, Title = "Open orders", X = 0, Y = 7, W = 12, H = 9,
                Query = new WidgetQuery { EntitySet = Orders, Filter = Open, Columns = ["Number", "Customer/Name", "Channel", "Region", "OrderDate", "Items", "Total"] },
            },
        ],
    };

    private static ReportDefinition Catalog() => new()
    {
        Widgets =
        [
            new ReportWidget
            {
                Id = "products", Type = WidgetType.Kpi, Title = "Products", X = 0, Y = 0, W = 3, H = 3,
                Query = new WidgetQuery { EntitySet = "Products", Aggregates = [new()] }, Options = new WidgetOptions { Format = NumberFormat.Integer },
            },
            new ReportWidget
            {
                Id = "rate", Type = WidgetType.Kpi, Title = "Average rate", X = 3, Y = 0, W = 3, H = 3,
                Query = new WidgetQuery { EntitySet = "Products", Aggregates = [new(AggregateFunction.Average, "Rate")] }, Options = new WidgetOptions { Format = NumberFormat.Currency },
            },
            new ReportWidget
            {
                Id = "brands", Type = WidgetType.Kpi, Title = "Brands", X = 6, Y = 0, W = 3, H = 3,
                Query = new WidgetQuery { EntitySet = "Brands", Aggregates = [new()] }, Options = new WidgetOptions { Format = NumberFormat.Integer },
            },
            new ReportWidget
            {
                Id = "note", Type = WidgetType.Markdown, Title = "Catalog", X = 9, Y = 0, W = 3, H = 3,
                Options = new WidgetOptions { Text = "Product managers keep products, brand managers keep brands. Click a brand to filter the products." },
            },
            new ReportWidget
            {
                Id = "per-brand", Type = WidgetType.Chart, Title = "Products per brand", X = 0, Y = 3, W = 6, H = 6,
                Query = new WidgetQuery { EntitySet = "Products", GroupBy = new ReportGroupBy("Brand/Name"), Aggregates = [new()], SortBy = ReportSortBy.Value, Descending = true },
                Options = new WidgetOptions { Chart = ChartKind.Bar, Format = NumberFormat.Integer },
            },
            new ReportWidget
            {
                Id = "value", Type = WidgetType.Chart, Title = "Rates by brand", X = 6, Y = 3, W = 6, H = 6,
                Query = new WidgetQuery { EntitySet = "Products", GroupBy = new ReportGroupBy("Brand/Name"), Aggregates = [new(AggregateFunction.Sum, "Rate")] },
                Options = new WidgetOptions { Chart = ChartKind.Treemap, Format = NumberFormat.Currency },
            },
            new ReportWidget
            {
                Id = "cards", Type = WidgetType.Table, Title = "Products", X = 0, Y = 9, W = 12, H = 6,
                Query = new WidgetQuery { EntitySet = "Products", Columns = ["Name", "Brand/Name", "Rate", "Barcode", "CreatedAt"], OrderBy = "Name" },
                Options = new WidgetOptions { View = TableView.Cards },
            },
        ],
    };
}
