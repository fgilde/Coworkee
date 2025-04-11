using Aspire.Hosting;
using Coworkee.AppHost.Types;
using Coworkee.Shared.Constants.Application;

namespace Coworkee.AppHost.ApplicationServiceHelper;

internal static class AspireMainAppHelper
{
    public static IList<IResourceBuilder<IResource>> AddDependencyServices(this IDistributedApplicationBuilder builder, IAppHostServiceSettings aspireServiceSettings )
    {
        var administrator = ApplicationConstants.Defaults.Users.Administrators[0];
        var userNameParam = builder.AddParameter("AdminUserName", administrator.UserName, true);
        var userPasswordParam = builder.AddParameter("AdminUserPassword", administrator.Password, true);

        return [
            .. builder.AddSignalRIf(builder.ExecutionContext.IsPublishMode),
            .. builder.AddDatabaseIf(true, aspireServiceSettings.DatabaseToUse),
            .. builder.AddKeycloakIf(aspireServiceSettings.AddKeycloak, ApplicationConstants.ServiceNames.Keycloak, userNameParam, userPasswordParam),
            .. builder.AddAzureStorageIf(aspireServiceSettings.AddAzureStorage),
            .. builder.AddOllamaIf(aspireServiceSettings.AddOllama),
            .. builder.AddGrafanaIf(aspireServiceSettings.AddGrafana, userNameParam, userPasswordParam, administrator),
            .. builder.AddStirlingIf(aspireServiceSettings.AddStirling),
        ];
    }
}