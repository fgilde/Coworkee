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

    public static IServiceCollection AddOrderApproval(this IServiceCollection services) =>
        services.AddCoworkeeWorkflows(workflows => workflows.Entity<SalesOrder>(EntityType, order =>
        {
            order.Initial("Open").Describe(o => o.Number).Link(_ => "/sales/orders");
            order.Transition("Confirm", "Open", "Confirmed").RequiresPermission(SalesPermissions.Orders.Edit)
                .When(o => o.Status == SalesOrderStatus.Open && o.Total < LargeTotal);
            order.Transition("Request approval", "Open", "Awaiting approval").RequiresPermission(SalesPermissions.Orders.Edit)
                .When(o => o.Status == SalesOrderStatus.Open && o.Total >= LargeTotal);
            order.State("Awaiting approval").Approval("Confirmed", "Declined", approval => approval
                .Role("Sales Manager", "admin")
                .DueIn(TimeSpan.FromDays(2))
                .RemindAfter(TimeSpan.FromDays(1)));
            order.State("Confirmed").Set(o => o.Status = SalesOrderStatus.Confirmed);
            order.State("Declined").Set(o => o.Status = SalesOrderStatus.Cancelled);
        }));
}
