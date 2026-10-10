using FluentValidation;
using MyApp.Contracts.Sales;

namespace MyApp.Sales.Features.Orders;

internal sealed class AddEditSalesOrderValidator : AbstractValidator<AddEditSalesOrderRequest>
{
    public AddEditSalesOrderValidator()
    {
        RuleFor(o => o.CustomerId).NotEmpty();
        RuleFor(o => o.Status).IsInEnum();
        RuleFor(o => o.Channel).IsInEnum();
        RuleFor(o => o.Region).NotEmpty().MaximumLength(50);
        RuleFor(o => o.Items).GreaterThan(0);
        RuleFor(o => o.Total).GreaterThanOrEqualTo(0);
    }
}
