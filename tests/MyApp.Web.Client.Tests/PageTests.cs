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
using MyApp.Contracts.Sales;
using MyApp.Web.Client.Api;
using MyApp.Web.Client.Pages;
using MyApp.Web.Client.Pages.Catalog;
using MyApp.Web.Client.Pages.Documents;
using MyApp.Web.Client.Pages.Sales;
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
        Services.AddSingleton(Substitute.For<IDocumentTypeAppService>());
        Services.AddSingleton(Substitute.For<ISalesOrderAppService>());
        Services.AddSingleton<IODataClient>(_odata);
        Services.AddSingleton<Coworkee.Client.Blazor.ClientEntities.IClientEntityStorage, Coworkee.Client.Blazor.ClientEntities.NoClientEntityStorage>();
        Coworkee.Client.Blazor.ClientEntities.ClientEntitiesServiceCollectionExtensions.AddCoworkeeClientEntities(Services);
        // no server: the client entity manifest fails at once and every query goes to the fake OData client
        Services.ConfigureHttpClientDefaults(client => client.ConfigurePrimaryHttpMessageHandler(() => new NoServer()));
        Services.AddSingleton(Substitute.For<ICoworkeeApi>());
        Services.AddSingleton(new CoworkeeClientOptions());
        Services.AddSingleton(Substitute.For<IRealtimeConnection>());
        Services.AddScoped<RealtimeClient>();
        Services.AddScoped<FileDownloader>();
        Services.AddScoped<ITableViewStore, TableViews>();
        Services.AddScoped<Coworkee.Client.Blazor.Security.PermissionStore>();
        Services.AddScoped<Coworkee.Client.Blazor.People.UserCards>();
        var localization = Substitute.For<Coworkee.Client.Blazor.Localization.ILocalizationApi>();
        localization.GetTextsAsync(default!, default).ReturnsForAnyArgs(call => new Coworkee.Contracts.Localization.TextsDto(call.Arg<string>(), new Dictionary<string, string>()));
        Services.AddSingleton(localization);
        Services.AddScoped<Coworkee.Client.Blazor.Localization.CoworkeeLocalizer>();
        AddAuthorization().SetAuthorized("Ada").SetPolicies(
            PermissionPolicy.For(CatalogPermissions.Brands.Delete), PermissionPolicy.For(DocumentPermissions.Documents.View), PermissionPolicy.For(SalesPermissions.Orders.Edit));
    }

    [Fact]
    public async Task Brands_come_from_odata_and_are_deleted_after_confirmation()
    {
        var acme = new BrandDto { Id = Guid.CreateVersion7(), Name = "Acme", Description = "Tools", Tax = 19 };
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
    public async Task A_pdf_is_previewed_in_the_shared_preview_and_the_list_follows_realtime()
    {
        var pdf = new DocumentDto(Guid.CreateVersion7(), "Offer", null, true, null, null, "offer.pdf", "application/pdf", 2048, null, DateTimeOffset.UtcNow);
        _odata.With("Documents", pdf);
        var dialogs = Render<MudDialogProvider>();
        Render<MudPopoverProvider>();
        var page = Render<DocumentStore>();

        await page.WaitForElement("[data-testid='preview-row']").ClickAsync(new());

        dialogs.WaitForAssertion(() => dialogs.Find("[data-testid='file-preview']").InnerHtml.ShouldContain("mud-ex-file-display-pdf"));
        dialogs.FindComponent<MudBlazor.Extensions.Components.MudExFileDisplay>().Instance.Url.ShouldBe($"http://localhost/api/v1/documents/{pdf.Id}/content");
        page.FindComponent<Coworkee.Client.Blazor.Components.RealtimeSubscription>().Instance.Topic.ShouldBe("type:Document");
        page.FindAll("[data-testid='delete-row']").ShouldBeEmpty();
        DocumentStore.FormatSize(2048).ShouldBe("2 KB");
    }

    [Fact]
    public async Task Sales_orders_are_listed_cancelled_through_the_app_service_and_compared_with_the_server()
    {
        var order = new SalesOrderDto(Guid.CreateVersion7(), "SO-0000042", Guid.CreateVersion7(), new CustomerDto(Guid.CreateVersion7(), "Hanse Logistik KG", "Bremen"),
            SalesOrderStatus.Open, SalesChannel.Online, "Nord", DateTimeOffset.UtcNow, 2, 19.80m);
        _odata.With("SalesOrders", order);
        Render<MudPopoverProvider>();
        var page = Render<SalesOrders>();

        page.WaitForAssertion(() => page.Markup.ShouldContain("Hanse Logistik KG"));
        await page.Find("[data-testid='cancel-row']").ClickAsync(new());
        await page.Find("[data-testid='compare-query']").ClickAsync(new());

        await Services.GetRequiredService<ISalesOrderAppService>().Received().CancelAsync(order.Id, Arg.Any<CancellationToken>());
        page.WaitForAssertion(() => page.Find("[data-testid='query-comparison']").TextContent.ShouldContain("Total desc"));
    }

    private sealed class NoServer : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(System.Net.HttpStatusCode.NotFound));
    }
}
