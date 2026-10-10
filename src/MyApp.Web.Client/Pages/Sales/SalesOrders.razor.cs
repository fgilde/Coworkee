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
    private static readonly string[] ComparedOrders = ["Total desc", "OrderDate desc", "Customer/Name", "Number desc"];
    private static readonly AggregateDefinition<SalesOrderDto> TotalSum = new() { Type = AggregateType.Sum, DisplayFormat = "Σ {value}", NumberFormat = "N2" };

    private CoworkeeDataTable<SalesOrderDto> _table = null!;
    private string? _comparison;
    private bool _comparing;
    private int _compared;

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

    // another sort order each time, so neither side answers from what it sorted just before
    private async Task CompareAsync()
    {
        _comparing = true;
        try
        {
            var orderBy = ComparedOrders[_compared++ % ComparedOrders.Length];
            var query = new ODataQuery { Filter = _table.CurrentFilter, OrderBy = orderBy, Expand = Expand, Top = 25, Facets = true };
            var server = await TimeAsync(() => Server.QueryAsync<SalesOrderDto>(EntitySet, query));
            _comparison = await Local.SetAsync<SalesOrderDto>(EntitySet, Expand) is { IsLocal: true } set
                ? L["Sorted by {0}: server {1} ms, browser {2} ms", orderBy, server, await TimeAsync(() => set.QueryAsync(query))]
                : L["Sorted by {0}: server {1} ms, the browser is still loading", orderBy, server];
        }
        finally
        {
            _comparing = false;
        }
    }

    private static async Task<long> TimeAsync(Func<Task> query)
    {
        var watch = Stopwatch.StartNew();
        await query();
        return watch.ElapsedMilliseconds;
    }
}
