using Coworkee.Application.Configurations;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Models;

namespace Coworkee.AppHost.Helper;

internal static class DbHelper
{
    public static IEnumerable<IResourceBuilder<IResource>> WithDatabaseIf(this IDistributedApplicationBuilder builder, bool condition, DatabaseToUse databaseToUse,
            string? serviceName = null, CreateUser? administrator = null)
    {
        if(!condition)
            yield break;
        var res = WithDatabase(builder, databaseToUse, serviceName, administrator);
        yield return res.DatabaseResource;
        if (res.AdminUIResource != null)
            yield return res.AdminUIResource;
    }
    

    public static (IResourceBuilder<IResourceWithConnectionString> DatabaseResource, IResourceBuilder<ContainerResource>? AdminUIResource) WithDatabase(this IDistributedApplicationBuilder builder, DatabaseToUse databaseToUse, 
        string? serviceName = null, CreateUser? administrator = null )
    {
        bool addDataVolume = !builder.ExecutionContext.IsRunMode;
        administrator ??= ApplicationConstants.Defaults.Users.Administrators.First();
        IResourceBuilder<ContainerResource> adminUiResource = null;
        IResourceBuilder<IResourceWithConnectionString> resourceResult = databaseToUse switch
        {
            DatabaseToUse.Postgres => builder.AddPostgres(serviceName ?? ApplicationConstants.ServiceNames.Postgress //, dbUsername, dbPassword
                ).PublishAsContainer()
                .WithDataVolumeIf(addDataVolume, $"{ApplicationConstants.ApplicationName}{ApplicationConstants.ServiceNames.Postgress}Data")
                .WithPgAdmin(admin =>
                {
                    admin.WithEnvironment("PGADMIN_CONFIG_SERVER_MODE", "True");
                    admin.WithEnvironment("PGADMIN_DEFAULT_EMAIL", administrator.Email);
                    admin.WithEnvironment("PGADMIN_DEFAULT_PASSWORD", administrator.Password);
                    admin.WithExternalHttpEndpoints().PublishAsContainer();
                    adminUiResource = admin;
                }, containerName: ApplicationConstants.ServiceNames.PgAdmin)
                .WithLifetime(ContainerLifetime.Persistent)
                .AddDatabase(nameof(ServerConfiguration.ConnectionStrings.DefaultConnection), ApplicationConstants.DatabaseName),
            DatabaseToUse.SqlServer => builder.AddSqlServer(serviceName ?? ApplicationConstants.ServiceNames.SqlServer)
                .WithDataVolumeIf(addDataVolume, $"{ApplicationConstants.ApplicationName}{ApplicationConstants.ServiceNames.SqlServer}Data")
                .WithLifetime(ContainerLifetime.Persistent)
                .AddDatabase(nameof(ServerConfiguration.ConnectionStrings.DefaultConnection))
        };

        return (resourceResult, adminUiResource);
    }
}