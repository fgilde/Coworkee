using System.Diagnostics;
using Microsoft.Playwright;

namespace Application.UITests.Base;

[Collection("UI Test Collection")]
public abstract class UiTestBase : IAsyncLifetime
{
    const float DEFAULT_TIMEOUT = 30000; // Default timeout in Playwright milliseconds
    public const float TIMEOUT = DEFAULT_TIMEOUT * 6; 
    protected readonly UITestFixture Fixture;
    protected string FrontendUrl => Fixture.Url;
    protected IPlaywright Playwright { get; private set; }
    protected IBrowser Browser { get; private set; }
    protected IPage Page { get; private set; }

    protected UiTestBase(UITestFixture fixture)
    {
        Fixture = fixture;
    }

    public virtual async Task InitializeAsync()
    {
        Playwright = await Microsoft.Playwright.Playwright.CreateAsync();
        Browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Timeout = TIMEOUT * 6, 
            Headless = !Debugger.IsAttached, 
            SlowMo = 100  
        });
        
        var context = await Browser.NewContextAsync();
        Page = await context.NewPageAsync();
    }

    public virtual async Task DisposeAsync()
    {
        if (Page != null)
            await Page.CloseAsync();

        if (Browser != null)
            await Browser.CloseAsync();

        Playwright?.Dispose();
    }
}

