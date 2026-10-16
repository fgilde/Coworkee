using Coworkee.Workflows;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Contracts.Sales;
using MyApp.Sales.Domain;

namespace MyApp.Sales.Features.Orders;

/// <summary>Open orders are confirmed directly, large ones (from <see cref="LargeTotal"/>) only after a sales manager or an administrator approved them within two days.</summary>
public static class OrderApproval
{
    public const string EntityType = "SalesOrders";

    public const decimal LargeTotal = 2000m;

    public const string Initial = "Open";

    public static IServiceCollection AddOrderApproval(this IServiceCollection services) =>
        services.AddCoworkeeWorkflows(workflows => workflows.Entity<SalesOrder>(EntityType, order =>
        {
            // the browser keeps the orders (client entity) and filters them itself, so the state is mirrored into a column
            order.Initial(Initial).Describe(o => o.Number).Link(_ => "/sales/orders").MirrorTo(o => o.ApprovalState);
            order.Transition("Confirm", "Open", "Confirmed").RequiresPermission(SalesPermissions.Orders.Edit)
                .When(o => o.Status == SalesOrderStatus.Open && o.Total < LargeTotal);
            order.Transition("Request approval", "Open", "Awaiting approval").RequiresPermission(SalesPermissions.Orders.Edit)
                .When(o => o.Status == SalesOrderStatus.Open && o.Total >= LargeTotal);
            order.State("Awaiting approval").Approval("Confirmed", "Declined", approval => approval
                .Role("Sales Manager", "admin")
                .DueIn(TimeSpan.FromDays(2))
                .RemindAfter(TimeSpan.FromDays(1)));
            order.State("Awaiting approval").WithColor("var(--mud-palette-warning)");
            order.State("Confirmed").WithColor("var(--mud-palette-success)").Set(o => o.Status = SalesOrderStatus.Confirmed);
            order.State("Declined").WithColor("var(--mud-palette-error)").Set(o => o.Status = SalesOrderStatus.Cancelled);
        }));
}
