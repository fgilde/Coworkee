using System.Net.Http.Json;
using System.Text.Json;
using Aspire.Hosting;
using Aspire.Hosting.Testing;

// the app hosts share the Grafana configs they generate under src/MyApp.AppHost/observability, so they start one after the other
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly)]

namespace MyApp.AppHost.Tests;

public sealed class AppHostTests
{
    [Fact]
    public async Task Api_starts_after_migrations_and_serves_system_info()
    {
        var ct = TestContext.Current.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MyApp_AppHost>([$"--{CoworkeeAppExtensions.EphemeralSetting}=true"], ct);
        var app = await appHost.BuildAsync(ct);
        await using var stop = AppHostDiagnostics.Guard(app);
        await app.StartAsync(ct);

        await AppHostDiagnostics.WaitHealthyAsync(app, ["myapp-api"], ct);
        using var client = app.CreateHttpClient("myapp-api");
        var info = await client.GetFromJsonAsync<JsonElement>("/api/v1/system/info", ct);

        info.GetProperty("product").GetString().ShouldBe("MyApp");
    }

    [Fact]
    public async Task Web_airworks_and_auth_start_and_serve()
    {
        var ct = TestContext.Current.CancellationToken;
        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.MyApp_AppHost>([$"--{CoworkeeAppExtensions.EphemeralSetting}=true"], ct);
        var app = await appHost.BuildAsync(ct);
        await using var stop = AppHostDiagnostics.Guard(app);
        await app.StartAsync(ct);
        await AppHostDiagnostics.WaitHealthyAsync(app, ["myapp-web", "myapp-airworks", "myapp-auth"], ct);

        using var web = app.CreateHttpClient("myapp-web");
        (await web.GetAsync("/", ct)).StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
        (await web.GetStringAsync("/bff/user", ct)).ShouldContain("\"isAuthenticated\":false");
        using var airWorks = app.CreateHttpClient("myapp-airworks");
        (await airWorks.GetStringAsync("/", ct)).ShouldContain("<title>MyApp</title>");
        (await airWorks.GetStringAsync("/bff/user", ct)).ShouldContain("\"isAuthenticated\":false");
        using var auth = app.CreateHttpClient("myapp-auth");
        (await auth.GetAsync("/.well-known/openid-configuration", ct)).StatusCode.ShouldBe(System.Net.HttpStatusCode.OK);
    }
}
