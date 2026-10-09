using Coworkee.Application.Registration;
using Coworkee.Core.Modularity;
using Coworkee.Infrastructure.Persistence;
using Coworkee.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MyApp.Documents.Persistence;

namespace MyApp.Documents.Registration;

/// <summary>For the auth server: keeps registration documents in the documents of the app, without the documents api.</summary>
[DependsOn(typeof(CoworkeeStorageModule))]
public sealed class MyAppRegistrationDocumentsModule : CoworkeeModule
{
    public override void ConfigureServices(ModuleServiceContext context)
    {
        context.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IModelContributor, DocumentModelContributor>());
        context.Services.TryAddSingleton(TimeProvider.System);
        context.Services.AddScoped<IRegistrationDocumentStore, DocumentRegistrationStore>();
    }
}
