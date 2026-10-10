using System.Diagnostics;
using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.ClientEntities;
using Coworkee.Client.Blazor.Components.Data;
using Coworkee.Client.Blazor.Data;
using Coworkee.Client.Blazor.Localization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using MyApp.Contracts;
using MyApp.Contracts.Sales;

namespace MyApp.Web.Client.Pages.Sales;

/// <summary>
/// Reads through the OData set "SalesOrders", which the browser mirrors as a client entity, and writes through the generated proxy of
/// <see cref="ISalesOrderAppService"/>; after a write the local copy fetches the change at once.
/// </summary>
public partial class SalesOrders
{
    private const string EntitySet = "SalesOrders";
    private const string Expand = "Customer";

    private static readonly string[] SearchFields = [nameof(SalesOrderDto.Number), "Customer/Name", nameof(SalesOrderDto.Region)];
    private static readonly string[] ComparedOrders = ["Total desc", "OrderDate desc", "Customer/Name", "Number desc", "Items desc"];
    private static readonly AggregateDefinition<SalesOrderDto> TotalSum = new() { Type = AggregateType.Sum, DisplayFormat = "Σ {value}", NumberFormat = "N2" };

    private CoworkeeDataTable<SalesOrderDto> _table = null!;
    private string? _comparison;
    private bool _comparing;

    [Inject] private CoworkeeLocalizer L { get; set; } = null!;

    [Inject] private ISalesOrderAppService Orders { get; set; } = null!;

    [Inject] private ClientEntities Local { get; set; } = null!;

    [Inject(Key = ClientEntitiesServiceCollectionExtensions.ServerKey)] private IODataClient Server { get; set; } = null!;

    [Inject] private IDialogService Dialogs { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private static Color StatusColor(SalesOrderStatus status) => status switch
    {
        SalesOrderStatus.Open => Color.Info,
        SalesOrderStatus.Confirmed => Color.Primary,
        SalesOrderStatus.Shipped => Color.Secondary,
        SalesOrderStatus.Delivered => Color.Success,
        _ => Color.Default,
    };

    private Task CreateAsync() => EditAsync(null, new AddEditSalesOrderRequest());

    private Task EditAsync(SalesOrderDto order) => EditAsync(order.Id, new AddEditSalesOrderRequest
    {
        CustomerId = order.CustomerId,
        Status = order.Status,
        Channel = order.Channel,
        Region = order.Region,
        Items = order.Items,
        Total = order.Total,
    });

    private async Task EditAsync(Guid? id, AddEditSalesOrderRequest model)
    {
        if (await Dialogs.ShowEditAsync(L[id is null ? "New order" : "Edit order"], model,
                saved => id is { } existing ? Orders.UpdateAsync(existing, saved) : Orders.CreateAsync(saved), Configure))
        {
            Snackbar.Add(L["Order saved"], Severity.Success);
        }
    }

#pragma warning disable BL0005 // MudEx configures the wrapping grid items through these instances
    private void Configure(ObjectEditMeta<AddEditSalesOrderRequest> meta)
    {
        meta.Property(o => o.CustomerId).WithLabel(L["Customer"]).WithOrder(0)
            .RenderWith<ODataPicker, Guid>(p => p.Value)
            .WithAdditionalAttribute(nameof(ODataPicker.EntitySet), "Customers")
            .WithAdditionalAttribute(nameof(ODataPicker.Required), true);
        meta.Property(o => o.Status).WithLabel(L["Status"]).WithOrder(1);
        meta.Property(o => o.Channel).WithLabel(L["Channel"]).WithOrder(2);
        meta.Property(o => o.Region).WithLabel(L["Region"]).WithOrder(3);
        meta.Property(o => o.Items).WithLabel(L["Items"]).WithOrder(4);
        meta.Property(o => o.Total).WithLabel(L["Total"]).WithOrder(5);
    }
#pragma warning restore BL0005

    private Task CancelAsync(SalesOrderDto order) => Snackbar.RunAsync(() => Orders.CancelAsync(order.Id));

    private Task DeleteAsync(IReadOnlyCollection<SalesOrderDto> orders) => Snackbar.RunAsync(() => Orders.DeleteAsync(new IdsRequest([.. orders.Select(o => o.Id)])));

    // the same queries on both sides: the current filter with facets, each sorted by another column, so neither answers from its last result
    private async Task CompareAsync()
    {
        _comparing = true;
        try
        {
            if (await Local.SetAsync<SalesOrderDto>(EntitySet, Expand) is not { IsLocal: true } set)
            {
                _comparison = L["The browser is still loading the orders"];
                return;
            }

            // plus a condition every row meets, new for each query, so neither side answers from a result it kept
            var queries = ComparedOrders.Select(o => new ODataQuery
            {
                Filter = ODataFilter.And(_table.CurrentFilter, $"Number ne '{Guid.NewGuid():N}'"), OrderBy = o, Expand = Expand, Top = 25, Facets = true,
            }).ToList();
            var server = await MedianAsync(queries, q => Server.QueryAsync<SalesOrderDto>(EntitySet, q));
            var browser = await MedianAsync(queries, q => set.QueryAsync(q));
            _comparison = L["Median of {0} queries (current filter, facets, 25 rows, another sort each): server {1} ms, browser {2} ms", queries.Count, server, browser];
        }
        finally
        {
            _comparing = false;
        }
    }

    private static async Task<long> MedianAsync(IEnumerable<ODataQuery> queries, Func<ODataQuery, Task> run)
    {
        var times = new List<long>();
        foreach (var query in queries)
        {
            var watch = Stopwatch.StartNew();
            await run(query);
            times.Add(watch.ElapsedMilliseconds);
        }

        times.Sort();
        return times[times.Count / 2];
    }
}
