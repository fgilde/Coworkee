using Coworkee.Application.Authorization;
using Coworkee.Application.Messaging;
using Coworkee.AspNetCore;
using Coworkee.Core.Modularity;
using Coworkee.Infrastructure.Persistence;
using Coworkee.ExtendedAttributes;
using Coworkee.OData;
using Coworkee.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MyApp.Application.Dashboard;
using MyApp.Contracts.Documents;
using MyApp.Documents.Domain;
using MyApp.Documents.Endpoints;
using MyApp.Documents.Features.Dashboard;
using MyApp.Documents.Features.DocumentTypes.Commands.AddEdit;
using MyApp.Documents.Permissions;
using MyApp.Documents.Persistence;
using MyApp.Documents.Visibility;

namespace MyApp.Documents;

[DependsOn(typeof(CoworkeeStorageModule), typeof(CoworkeeODataModule), typeof(CoworkeeExtendedAttributesModule))]
public sealed class MyAppDocumentsModule : CoworkeeModule, IWebModule
{
    public const long MaxSize = 100L * 1024 * 1024;

    public override void ConfigureServices(ModuleServiceContext context)
    {
        var services = context.Services;
        services.AddMessagingFromAssembly(typeof(MyAppDocumentsModule).Assembly);
        services.AddSingleton<IModelContributor, DocumentModelContributor>();
        services.AddSingleton<IPermissionDefinitionContributor, DocumentPermissionDefinitions>();
        services.AddScoped<DocumentVisibility>();
        services.AddScoped<IODataEntityFilter<Document>, DocumentODataFilter>();
        services.AddScoped<IDashboardCounts, DocumentDashboardCounts>();
        services.AddODataEntity<Document>("Documents", DocumentPermissions.Documents.View, d => d.BlobKey);
        services.AddODataEntity<DocumentType>("DocumentTypes", DocumentPermissions.Types.View);
        services.AddExtendedAttributes<Document>("Documents", DocumentPermissions.Documents.View, DocumentPermissions.Documents.Edit);
        services.AddODataImport("DocumentTypes", (AddEditDocumentTypeRequest row) => new AddEditDocumentTypeCommand(null, row));
    }

    public void ConfigureApplication(WebApplication app)
    {
        app.MapDocumentTypeEndpoints();
        app.MapDocumentEndpoints();
    }
}
