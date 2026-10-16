using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.AspNetCore.ApplicationServices;
using Coworkee.Core.Modularity;
using Coworkee.Infrastructure.Persistence;
using Coworkee.OData;
using Coworkee.OData.GlobalSearch;
using Coworkee.Webhooks;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Contracts.Sales;
using MyApp.Sales.Domain;
using MyApp.Sales.Features.Orders;
using MyApp.Sales.Permissions;
using MyApp.Sales.Persistence;

namespace MyApp.Sales;

/// <summary>
/// 100,000 sales orders to show client entities: the browser mirrors the OData set "SalesOrders" and answers the table, its facets,
/// grouping and search locally. Writes go through <see cref="ISalesOrderAppService"/>.
/// </summary>
[DependsOn(typeof(CoworkeeODataModule))]
public sealed class MyAppSalesModule : CoworkeeModule
{
    public override void ConfigureServices(ModuleServiceContext context)
    {
        var services = context.Services;
        services.AddMessagingFromAssembly(typeof(MyAppSalesModule).Assembly);
        services.AddSingleton<IModelContributor, SalesModelContributor>();
        services.AddSingleton<IPermissionDefinitionContributor, SalesPermissionDefinitions>();
        services.AddODataEntity<SalesOrder>("SalesOrders", SalesPermissions.Orders.View);
        services.AddODataEntity<Customer>("Customers", SalesPermissions.Orders.View);
        services.AddGlobalSearch<SalesOrder>(s => s.Fields(o => o.Number, o => o.Region).Subtitle(o => o.Region).Page("/sales/orders").Icon("ShoppingCart"));
        services.AddOrderApproval();
        services.AddCoworkeeWebhooks(webhooks => webhooks.Entity<SalesOrder>());

        // [ClientEntity] on SalesOrder mirrors it with the defaults (up to 50,000 rows); its service raises the limit for the demo's 100,000
        services.Configure<ApplicationServiceOptions>(options => options.Configure<ISalesOrderAppService>(service =>
            service.ClientEntity<SalesOrder>(settings => settings.MaxRows = 250_000)));
    }
}
