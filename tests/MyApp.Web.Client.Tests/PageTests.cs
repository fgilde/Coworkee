using Bunit;
using Coworkee.Client.Blazor;
using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.Data;
using Coworkee.Client.Blazor.Realtime;
using Coworkee.Client.Blazor.Security;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using MyApp.Contracts.Catalog;
using MyApp.Contracts.Documents;
using MyApp.Web.Client.Api;
using MyApp.Web.Client.Pages;
using MyApp.Web.Client.Pages.Catalog;
using MyApp.Web.Client.Pages.Documents;
using NSubstitute;

namespace MyApp.Web.Client.Tests;

public sealed class PageTests : BunitContext
{
    private readonly ICatalogApi _catalog = Substitute.For<ICatalogApi>();
    private readonly FakeODataClient _odata = new();

    public PageTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        MudBlazor.Extensions.ServiceCollectionExtensions.AddMudExtensions(Services);
        Services.AddSingleton(_catalog);
        Services.AddSingleton(Substitute.For<IDocumentsApi>());
        Services.AddSingleton<IODataClient>(_odata);
        Services.AddSingleton(Substitute.For<ICoworkeeApi>());
        Services.AddSingleton(new CoworkeeClientOptions());
        Services.AddSingleton(Substitute.For<IRealtimeConnection>());
        Services.AddScoped<RealtimeClient>();
        Services.AddScoped<FileDownloader>();
        Services.AddScoped<Coworkee.Client.Blazor.Security.PermissionStore>();
        Services.AddScoped<Coworkee.Client.Blazor.People.UserCards>();
        var localization = Substitute.For<Coworkee.Client.Blazor.Localization.ILocalizationApi>();
        localization.GetTextsAsync(default!, default).ReturnsForAnyArgs(call => new Coworkee.Contracts.Localization.TextsDto(call.Arg<string>(), new Dictionary<string, string>()));
        Services.AddSingleton(localization);
        Services.AddScoped<Coworkee.Client.Blazor.Localization.CoworkeeLocalizer>();
        AddAuthorization().SetAuthorized("Ada").SetPolicies(
            PermissionPolicy.For(CatalogPermissions.Brands.Delete), PermissionPolicy.For(DocumentPermissions.Documents.View));
    }

    [Fact]
    public async Task Brands_come_from_odata_and_are_deleted_after_confirmation()
    {
        var acme = new BrandDto(Guid.CreateVersion7(), "Acme", "Tools", 19);
        _odata.With("Brands", acme);
        var dialogs = Render<MudDialogProvider>();
        Render<MudPopoverProvider>();
        var page = Render<Brands>();

        page.WaitForAssertion(() => page.Markup.ShouldContain("Acme"));
        page.FindAll("[data-testid='edit-row']").ShouldBeEmpty();
        var deleting = page.Find("[data-testid='delete-row']").ClickAsync(new());
        dialogs.WaitForAssertion(() => dialogs.FindAll("button").Any(b => b.TextContent.Trim() == "Delete").ShouldBeTrue());
        await dialogs.FindAll("button").First(b => b.TextContent.Trim() == "Delete").ClickAsync(new());
        await deleting;

        await _catalog.Received(1).DeleteBrandsAsync(Arg.Is<IReadOnlyList<Guid>>(ids => ids.Single() == acme.Id), Arg.Any<CancellationToken>());
    }

    [Fact]
    public void The_dashboard_shows_the_counts()
    {
        _catalog.GetDashboardAsync(default).ReturnsForAnyArgs(new DashboardDto(3, 7, 2, 1, 5, 4, [new MonthCountDto(2026, 10, 7)]));

        var page = Render<Dashboard>();

        page.WaitForAssertion(() => page.Find("[data-stat='Registered Users']").TextContent.ShouldContain("5"));
        page.Find("[data-stat='Products']").TextContent.ShouldContain("7");
    }

    [Fact]
    public async Task A_pdf_is_previewed_with_mudex_file_display()
    {
        var pdf = new DocumentDto(Guid.CreateVersion7(), "Offer", null, true, null, null, "offer.pdf", "application/pdf", 2048, null, DateTimeOffset.UtcNow);
        _odata.With("Documents", pdf);
        Render<MudPopoverProvider>();
        var page = Render<DocumentStore>();

        await page.WaitForElement("[data-testid='preview-row']").ClickAsync(new());

        page.WaitForAssertion(() => page.Find("[data-testid='preview']").InnerHtml.ShouldContain("mud-ex-file-display-pdf"));
        page.FindAll("[data-testid='delete-row']").ShouldBeEmpty();
        DocumentStore.FormatSize(2048).ShouldBe("2 KB");
    }
}
