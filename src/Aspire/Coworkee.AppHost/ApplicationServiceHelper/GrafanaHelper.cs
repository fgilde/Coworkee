using Coworkee.AppHost.OpenTelemetryCollector;
using Coworkee.Shared.Models;
using C = Coworkee.Shared.Constants.Application.ApplicationConstants;

namespace Coworkee.AppHost.ApplicationServiceHelper;

internal static class GrafanaHelper
{
    public static IEnumerable<IResourceBuilder<IResource>> AddGrafanaIf(this IDistributedApplicationBuilder builder, bool condition, IResourceBuilder<ParameterResource> userNameParam,
        IResourceBuilder<ParameterResource> userPasswordParam, CreateUser administrator)
    {
        return !condition 
            ? [] 
            : AddGrafana(builder, userNameParam, userPasswordParam, administrator);
    }

    public static IResourceBuilder<ContainerResource>[] AddGrafana(this IDistributedApplicationBuilder builder, 
        IResourceBuilder<ParameterResource> userNameParam, 
        IResourceBuilder<ParameterResource> userPasswordParam, CreateUser administrator)
    {
        var prometheus = AddPrometheus(builder);

        var grafana = builder.AddContainer(C.ServiceNames.Grafana, "grafana/grafana")
            .WithDockerfile("grafana", "Dockerfile")
            .WithEnvironment("GF_SECURITY_ADMIN_USER", userNameParam)
            .WithEnvironment("GF_SECURITY_ADMIN_EMAIL", administrator.Email)
            .WithEnvironment("GF_SECURITY_ADMIN_PASSWORD", userPasswordParam)
            .WithEnvironment("GF_AUTH_ANONYMOUS_ENABLED", "false")
            .WithEnvironment("PROMETHEUS_ENDPOINT", prometheus.GetEndpoint("http"))
            .WithHttpEndpoint(targetPort: 3000, name: "http")
            .WithExternalHttpEndpoints();

        var collector = AddOtelCollector(builder, prometheus);
        return [prometheus, grafana, collector];
    }

    public static IResourceBuilder<OpenTelemetryCollectorResource> AddOtelCollector(IDistributedApplicationBuilder builder, IResourceBuilder<ContainerResource> prometheus)
    {
        return builder.AddOpenTelemetryCollector("otelcollector", "otelcollector/config.yaml")
            .WithEnvironment("PROMETHEUS_ENDPOINT", $"{prometheus.GetEndpoint("http")}/api/v1/otlp")
            .PublishAsContainer();
    }

    public static IResourceBuilder<ContainerResource> AddPrometheus(IDistributedApplicationBuilder builder)
    {
        return builder.AddContainer(C.ServiceNames.Prometheus, "prom/prometheus")
            .WithDockerfile("prometheus", "Dockerfile")
            .WithArgs("--web.enable-otlp-receiver", "--config.file=/etc/prometheus/prometheus.yml")
            .WithHttpEndpoint(targetPort: 9090, name: "http")
            .WithExternalHttpEndpoints();
    }
}