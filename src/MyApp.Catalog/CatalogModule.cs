using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.AspNetCore;
using Coworkee.Core.Modularity;
using Coworkee.Infrastructure.Persistence;
using Coworkee.OData;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Catalog.Domain;
using MyApp.Catalog.Features.Brands.Commands.AddEdit;
using MyApp.Catalog.Features.Products.Commands.AddEdit;
using MyApp.Catalog.Endpoints;
using MyApp.Catalog.Permissions;
using MyApp.Catalog.Persistence;
using MyApp.Contracts.Catalog;

namespace MyApp.Catalog;

[DependsOn(typeof(CoworkeeODataModule))]
public sealed class MyAppCatalogModule : CoworkeeModule, IWebModule
{
    public override void ConfigureServices(ModuleServiceContext context)
    {
        context.Services.AddMessagingFromAssembly(typeof(MyAppCatalogModule).Assembly);
        context.Services.AddSingleton<IModelContributor, CatalogModelContributor>();
        context.Services.AddSingleton<IPermissionDefinitionContributor, CatalogPermissionDefinitions>();
        context.Services.AddODataEntity<Brand>("Brands", CatalogPermissions.Brands.View);
        context.Services.AddODataEntity<Product>("Products", CatalogPermissions.Products.View);
        context.Services.AddODataImport("Brands", (AddEditBrandRequest row) => new AddEditBrandCommand(null, row));
        context.Services.AddODataImport("Products", (AddEditProductRequest row) => new AddEditProductCommand(null, row));
    }

    public void ConfigureApplication(WebApplication app)
    {
        app.MapBrandEndpoints();
        app.MapProductEndpoints();
        app.MapDashboardEndpoints();
    }
}
