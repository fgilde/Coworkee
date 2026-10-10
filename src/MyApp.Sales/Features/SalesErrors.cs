using Coworkee.Core.Results;

namespace MyApp.Sales.Features;

internal static class SalesErrors
{
    public static readonly Error OrderNotFound = Error.NotFound("sales.order_not_found", "The sales order does not exist.");

    public static readonly Error UnknownCustomer = Error.Validation("CustomerId", "The customer does not exist.");
}
