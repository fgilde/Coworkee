using System.Globalization;
using Coworkee.Application.Authorization;
using Coworkee.Core.Results;
using Coworkee.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MyApp.Contracts;
using MyApp.Contracts.Sales;
using MyApp.Sales.Domain;

namespace MyApp.Sales.Features.Orders;

/// <summary>Works on the DbContext directly: the endpoint validates the request first and saves the unit of work after.</summary>
internal sealed class SalesOrderAppService(CoworkeeDbContext db, TimeProvider clock) : ISalesOrderAppService
{
    [RequiresPermission(SalesPermissions.Orders.View)]
    public async Task<SalesOrderDto> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        (await db.Set<SalesOrder>().AsNoTracking().Include(o => o.Customer).SingleOrDefaultAsync(o => o.Id == id, cancellationToken))?.ToDto()
        ?? throw new ErrorException(SalesErrors.OrderNotFound);

    [RequiresPermission(SalesPermissions.Orders.Create)]
    public async Task<SalesOrderDto> CreateAsync(AddEditSalesOrderRequest input, CancellationToken cancellationToken = default)
    {
        var order = new SalesOrder { Number = await NextNumberAsync(cancellationToken), Region = input.Region, OrderDate = clock.GetUtcNow() };
        db.Add(order);
        return await SaveAsync(order, input, cancellationToken);
    }

    [RequiresPermission(SalesPermissions.Orders.Edit)]
    public async Task<SalesOrderDto> UpdateAsync(Guid id, AddEditSalesOrderRequest input, CancellationToken cancellationToken = default) =>
        await SaveAsync(await FindAsync(id, cancellationToken), input, cancellationToken);

    [RequiresPermission(SalesPermissions.Orders.Edit)]
    public async Task CancelAsync(Guid id, CancellationToken cancellationToken = default) =>
        (await FindAsync(id, cancellationToken)).Status = SalesOrderStatus.Cancelled;

    [RequiresPermission(SalesPermissions.Orders.Delete)]
    public async Task DeleteAsync(IdsRequest request, CancellationToken cancellationToken = default) =>
        db.RemoveRange(await db.Set<SalesOrder>().Where(o => request.Ids.Contains(o.Id)).ToListAsync(cancellationToken));

    private async Task<SalesOrderDto> SaveAsync(SalesOrder order, AddEditSalesOrderRequest input, CancellationToken cancellationToken)
    {
        order.Customer = await db.Set<Customer>().SingleOrDefaultAsync(c => c.Id == input.CustomerId, cancellationToken)
            ?? throw new ErrorException(SalesErrors.UnknownCustomer);
        order.Apply(input);
        return order.ToDto();
    }

    private async Task<SalesOrder> FindAsync(Guid id, CancellationToken cancellationToken) =>
        await db.Set<SalesOrder>().SingleOrDefaultAsync(o => o.Id == id, cancellationToken) ?? throw new ErrorException(SalesErrors.OrderNotFound);

    private async Task<string> NextNumberAsync(CancellationToken cancellationToken)
    {
        var last = await db.Set<SalesOrder>().MaxAsync(o => (string?)o.Number, cancellationToken);
        var next = last is null ? 1 : int.Parse(last.AsSpan(3), CultureInfo.InvariantCulture) + 1;
        return SalesOrderNumbers.Format(next);
    }
}
