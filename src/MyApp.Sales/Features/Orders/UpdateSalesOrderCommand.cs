using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Contracts.Sales;
using MyApp.Sales.Domain;

namespace MyApp.Sales.Features.Orders;

/// <summary>Changes an order like the app service does; the entity history restores earlier states with it.</summary>
[RequiresPermission(SalesPermissions.Orders.Edit)]
public sealed record UpdateSalesOrderCommand(Guid Id, AddEditSalesOrderRequest Order) : ICommand<Result>;

internal sealed class UpdateSalesOrderHandler(CoworkeeDbContext db) : IHandler<UpdateSalesOrderCommand, Result>
{
    public async Task<Result> HandleAsync(UpdateSalesOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await db.Set<SalesOrder>().SingleOrDefaultAsync(o => o.Id == command.Id, cancellationToken);
        if (order is null)
        {
            return SalesErrors.OrderNotFound;
        }

        if (!await db.Set<Customer>().AnyAsync(c => c.Id == command.Order.CustomerId, cancellationToken))
        {
            return SalesErrors.UnknownCustomer;
        }

        order.Apply(command.Order);
        return Result.Success();
    }
}
