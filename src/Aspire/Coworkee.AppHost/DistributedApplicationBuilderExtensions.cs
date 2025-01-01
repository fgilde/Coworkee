using Aspire.Hosting;
using Microsoft.Extensions.Configuration;

namespace Coworkee.AppHost;

public static class DistributedApplicationBuilderExtensions
{

    internal static IResourceBuilder<T> WaitForIf<T>(this IResourceBuilder<T> builder, IResourceBuilder<IResource>? dependency) where T : IResourceWithWaitSupport
    {
        return dependency is {Resource: IResourceWithParent} ? builder.WaitFor(dependency) : builder;
    }

    internal static IResourceBuilder<T> WaitForCompletionIf<T>(this IResourceBuilder<T> builder, IResourceBuilder<IResource>? dependency) where T : IResourceWithWaitSupport
    {
        return dependency is { Resource: IResourceWithParent } ? builder.WaitForCompletion(dependency) : builder;
    }

    internal static IResourceBuilder<T> WithReferenceIf<T>(this IResourceBuilder<T> builder, IResourceBuilder<IResourceWithConnectionString>? dependency) where T : IResourceWithEnvironment
    {
        return dependency != null ? builder.WithReference(dependency) : builder;
    }

    internal static IResourceBuilder<T> WithReferenceIf<T>(this IResourceBuilder<T> builder, IResourceBuilder<IResourceWithServiceDiscovery>? dependency) where T : IResourceWithEnvironment
    {
        return dependency != null ? builder.WithReference(dependency) : builder;
    }

    internal static IResourceBuilder<T> WithReferenceIf<T>(this IResourceBuilder<T> builder, string name, Uri? uri) where T : IResourceWithEnvironment
    {
        return uri != null ? builder.WithReference(name, uri) : builder;
    }

    internal static IResourceBuilder<T> WithReferenceIf<T>(this IResourceBuilder<T> builder, EndpointReference? dependency) where T : IResourceWithEnvironment
    {
        return dependency != null ? builder.WithReference(dependency) : builder;
    }
}
