using lib.Coworkee.Shared.Constants.Application;

namespace Coworkee.AppHost.ApplicationServiceHelper;

internal static class StirlingHelper
{
    public static IEnumerable<IResourceBuilder<IResource>> AddStirlingIf(this IDistributedApplicationBuilder builder,
        bool condition)
    {
       if(!condition)
           yield break;
        yield return builder.AddStirling();
    }

    public static IResourceBuilder<ContainerResource> AddStirling(this IDistributedApplicationBuilder builder)
    {
        return builder.AddContainer(ApplicationConstants.ServiceNames.Stirling, "stirlingtools/stirling-pdf")
            .WithHttpEndpoint(targetPort: 8080, name: "http")
            .WithHttpHealthCheck("/", 200)
            .WithExternalHttpEndpoints();
    }
}