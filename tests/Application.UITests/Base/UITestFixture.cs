using Aspire.Hosting;
using Aspire.Hosting.ApplicationModel;
using Aspire.Hosting.Testing;
using Coworkee.Shared.Constants.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UITests.Base;

public class UITestFixture : IAsyncLifetime
{
    private readonly string _overrideUrl;
    public string Url { get; private set; }
    public HttpClient HttpClient { get; private set; }
    public DistributedApplication AspireApplication { get; private set; }

    public UITestFixture()
    {
        _overrideUrl = Environment.GetEnvironmentVariable("UITEST_FRONTEND_URL");        
    }

    public async Task InitializeAsync()
    {
        var aspireResourceName = ApplicationConstants.HostClientInServer ? ApplicationConstants.AspireServerAppName : ApplicationConstants.AspireClientAppName;
        if (!string.IsNullOrWhiteSpace(_overrideUrl))
        {
            Console.WriteLine($"[TEST FIXTURE] Using external URL: {_overrideUrl}");
            Url = GetUrlWithOutSlash(_overrideUrl);
            return;
        }

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.Coworkee_AppHost>();

        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        AspireApplication = await appHost.BuildAsync();

        var resourceNotificationService = AspireApplication.Services
            .GetRequiredService<ResourceNotificationService>();

        await AspireApplication.StartAsync();

        HttpClient = AspireApplication.CreateHttpClient(aspireResourceName);

        await resourceNotificationService
            .WaitForResourceAsync(aspireResourceName, KnownResourceStates.Running)
            .WaitAsync(TimeSpan.FromMinutes(5));

        Url = GetUrlWithOutSlash(HttpClient.BaseAddress?.AbsoluteUri) ?? throw new Exception("Failed to start");

    }

    private string? GetUrlWithOutSlash(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return null;
        return url.EndsWith("/") ? url[..^1] : url;
    }

    public Task DisposeAsync() => AspireApplication == null ? Task.CompletedTask : AspireApplication.StopAsync();
}

