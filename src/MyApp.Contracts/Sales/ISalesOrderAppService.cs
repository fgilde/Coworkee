using Coworkee.Contracts.Services;

namespace MyApp.Contracts.Sales;

/// <summary>
/// The writes of sales orders, served under /api/v1/sales-orders by convention; the Blazor client gets a generated proxy.
/// Reading goes through the OData set "SalesOrders", which the browser keeps as a client entity.
/// </summary>
public interface ISalesOrderAppService : IApplicationService
{
    Task<SalesOrderDto> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SalesOrderDto> CreateAsync(AddEditSalesOrderRequest input, CancellationToken cancellationToken = default);

    Task<SalesOrderDto> UpdateAsync(Guid id, AddEditSalesOrderRequest input, CancellationToken cancellationToken = default);

    /// <summary>POST /api/v1/sales-orders/{id}/cancel: a name without a known prefix is a POST on the row.</summary>
    Task CancelAsync(Guid id, CancellationToken cancellationToken = default);

    [ServiceOperation(Method = "POST", Route = "delete")]
    Task DeleteAsync(IdsRequest request, CancellationToken cancellationToken = default);
}
