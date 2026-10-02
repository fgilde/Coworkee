using Coworkee.Core.Modularity;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Infrastructure;

public sealed class MyAppDbContextDesignFactory : IDesignTimeDbContextFactory<MyAppDbContext>
{
    public MyAppDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection([new($"ConnectionStrings:{MyAppInfrastructureModule.ConnectionStringName}", "Host=localhost;Database=myapp_design")])
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddCoworkeeModules<MyAppDatabaseModule>(configuration);
        return services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<MyAppDbContext>();
    }
}
