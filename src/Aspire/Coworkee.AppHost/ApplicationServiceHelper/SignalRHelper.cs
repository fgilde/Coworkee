using lib.Coworkee.Shared.Constants.Application;

namespace Coworkee.AppHost.ApplicationServiceHelper;

public static class SignalRHelper
{
    public static IEnumerable<IResourceBuilder<IResource>> AddSignalRIf(this IDistributedApplicationBuilder builder,
        bool condition)
    {
        if (!condition)
            yield break;
        yield return builder.AddSignalR();
    }

    public static IResourceBuilder<AzureSignalRResource> AddSignalR(this IDistributedApplicationBuilder builder)
    {
        return builder.AddAzureSignalR(ApplicationConstants.SignalR.Resource);
    }
}