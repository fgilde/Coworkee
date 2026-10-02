using Coworkee.Application;
using Coworkee.Application.Messaging;
using Coworkee.Core.Modularity;

namespace MyApp.Application;

[DependsOn(typeof(CoworkeeApplicationModule))]
public sealed class MyAppApplicationModule : CoworkeeModule
{
    public override void ConfigureServices(ModuleServiceContext context) =>
        context.Services.AddMessagingFromAssembly(typeof(MyAppApplicationModule).Assembly);
}
