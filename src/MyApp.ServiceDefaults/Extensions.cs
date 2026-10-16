using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Microsoft.Extensions.Hosting;

public static class Extensions
{
    private const string HealthEndpointPath = "/health";
    private const string AlivenessEndpointPath = "/alive";

    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.ConfigureOpenTelemetry();
        builder.Services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy(), ["live"]);
        builder.Services.AddServiceDiscovery();
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            http.AddStandardResilienceHandler();
            http.AddServiceDiscovery();
        });
        return builder;
    }

    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        app.MapHealthChecks(HealthEndpointPath);
        app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions { Predicate = r => r.Tags.Contains("live") });
        return app;
    }

    private static void ConfigureOpenTelemetry<TBuilder>(this TBuilder builder)
        where TBuilder : IHostApplicationBuilder
    {
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
        });

        var telemetry = builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics => metrics.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddRuntimeInstrumentation()
                .AddMeter("Microsoft.EntityFrameworkCore", "Npgsql"))
            .WithTracing(tracing => tracing
                .AddSource(builder.Environment.ApplicationName)
                .AddAspNetCoreInstrumentation(o => o.Filter = ctx =>
                    !ctx.Request.Path.StartsWithSegments(HealthEndpointPath) && !ctx.Request.Path.StartsWithSegments(AlivenessEndpointPath))
                .AddHttpClientInstrumentation());

        // the Aspire dashboard, and the Grafana stack of the app host when it runs one
        if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
        {
            telemetry.WithLogging(l => l.AddOtlpExporter()).WithMetrics(m => m.AddOtlpExporter()).WithTracing(t => t.AddOtlpExporter());
        }

        if (builder.Configuration["GRAFANA_OTLP_ENDPOINT"] is { Length: > 0 } grafana)
        {
            // a deployed collector takes OTLP over HTTP, where each signal has its own path
            var http = builder.Configuration["GRAFANA_OTLP_PROTOCOL"] == "http/protobuf";
            Action<OtlpExporterOptions> Grafana(string signal) => o => (o.Endpoint, o.Protocol, o.Headers) = http
                ? (new Uri($"{grafana.TrimEnd('/')}/v1/{signal}"), OtlpExportProtocol.HttpProtobuf, string.Empty)
                : (new Uri(grafana), OtlpExportProtocol.Grpc, string.Empty);
            telemetry.WithLogging(l => l.AddOtlpExporter("grafana-logs", Grafana("logs"))).WithMetrics(m => m.AddOtlpExporter("grafana-metrics", Grafana("metrics")))
                .WithTracing(t => t.AddOtlpExporter("grafana-traces", Grafana("traces")));
        }
    }
}
