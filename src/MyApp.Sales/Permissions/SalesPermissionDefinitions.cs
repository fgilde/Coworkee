using Coworkee.Application.Authorization;
using MyApp.Contracts.Sales;

namespace MyApp.Sales.Permissions;

internal sealed class SalesPermissionDefinitions : IPermissionDefinitionContributor
{
    public void Define(PermissionDefinitionContext context) =>
        context.Group(SalesPermissions.GroupName, "Sales")
            .Add(SalesPermissions.Orders.View, "View sales orders")
            .Add(SalesPermissions.Orders.Create, "Create sales orders", SalesPermissions.Orders.View)
            .Add(SalesPermissions.Orders.Edit, "Edit and cancel sales orders", SalesPermissions.Orders.View)
            .Add(SalesPermissions.Orders.Delete, "Delete sales orders", SalesPermissions.Orders.View);
}
