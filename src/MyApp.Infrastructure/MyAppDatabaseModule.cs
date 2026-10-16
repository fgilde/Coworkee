using Coworkee.Auditing;
using Coworkee.Auditing.History;
using Coworkee.AuthServer;
using Coworkee.Core.Modularity;
using Coworkee.Localization;
using Coworkee.Localization.Resources;
using Coworkee.Mailing;
using Coworkee.Notifications;
using Coworkee.Theming;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Catalog.Domain;
using MyApp.Catalog.Features.Brands.Commands.AddEdit;
using MyApp.Catalog.Features.Products.Commands.AddEdit;
using MyApp.Contracts.Catalog;
using MyApp.Contracts.Sales;
using MyApp.Sales.Domain;
using MyApp.Sales.Features.Orders;

namespace MyApp.Infrastructure;

[DependsOn(typeof(MyAppInfrastructureModule), typeof(CoworkeeAuthStoreModule), typeof(CoworkeeMailingModule), typeof(CoworkeeAuditingModule), typeof(CoworkeeThemingModule),
    typeof(CoworkeeNotificationsModule), typeof(Coworkee.Account.CoworkeeAccountModule), typeof(CoworkeeLocalizationModule), typeof(Coworkee.Backup.CoworkeeBackupModule), typeof(Coworkee.Chat.CoworkeeChatModule), typeof(Coworkee.Reporting.CoworkeeReportingModule), typeof(Coworkee.Ai.CoworkeeAiModule), typeof(Coworkee.Features.CoworkeeFeaturesModule), typeof(Coworkee.Files.CoworkeeFilesModule), typeof(Coworkee.Social.CoworkeeSocialModule), typeof(Coworkee.Workflows.CoworkeeWorkflowsModule), typeof(Coworkee.Webhooks.CoworkeeWebhooksModule), typeof(MyApp.Catalog.MyAppCatalogModule),
    typeof(MyApp.Documents.MyAppDocumentsModule), typeof(MyApp.Sales.MyAppSalesModule))]
public sealed class MyAppDatabaseModule : CoworkeeModule
{
    public override void ConfigureServices(ModuleServiceContext context)
    {
        context.Services.AddSingleton<ILocalizationResourceContributor, MyAppTexts>();
        AddHistory(context.Services);
    }

    // the history of products, brands and orders, restored through their update commands
    private static void AddHistory(IServiceCollection services)
    {
        services.AddEntityHistory("Products", CatalogPermissions.Products.Edit, (Product p) => new AddEditProductCommand(p.Id, new AddEditProductRequest
        {
            Name = p.Name, Barcode = p.Barcode, Description = p.Description, ImageDataUrl = p.ImageDataUrl, Rate = p.Rate, BrandId = p.BrandId,
        }));
        services.AddEntityHistory("Brands", CatalogPermissions.Brands.Edit, (Brand b) => new AddEditBrandCommand(b.Id, new AddEditBrandRequest
        {
            Name = b.Name, Description = b.Description, Tax = b.Tax,
        }));
        services.AddEntityHistory("SalesOrders", SalesPermissions.Orders.Edit, (SalesOrder o) => new UpdateSalesOrderCommand(o.Id, new AddEditSalesOrderRequest
        {
            CustomerId = o.CustomerId, Status = o.Status, Channel = o.Channel, Region = o.Region, Items = o.Items, Total = o.Total, Options = o.Options, Paid = o.Paid,
        }));
    }
}
