using Coworkee.Application.Authorization;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Contracts.Sales;
using Nextended.Core.Attributes;

namespace MyApp.Sales.Permissions;

[RegisterAs(typeof(IPermissionDefinitionContributor), ServiceLifetime = ServiceLifetime.Singleton)]
internal sealed class SalesPermissionDefinitions : IPermissionDefinitionContributor
{
    public void Define(PermissionDefinitionContext context) =>
        context.Group(SalesPermissions.GroupName, "Sales")
            .Add(SalesPermissions.Orders.View, "View sales orders")
            .Add(SalesPermissions.Orders.Create, "Create sales orders", SalesPermissions.Orders.View)
            .Add(SalesPermissions.Orders.Edit, "Edit and cancel sales orders", SalesPermissions.Orders.View)
            .Add(SalesPermissions.Orders.Delete, "Delete sales orders", SalesPermissions.Orders.View);
}
