using Bunit;
using Coworkee.Client.Blazor;
using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.Realtime;
using Coworkee.Contracts;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using MyApp.Contracts.Catalog;
using MyApp.Contracts.Documents;
using MyApp.Web.Client.Pages;
using NSubstitute;

namespace MyApp.Web.Client.Tests;

public sealed class PageTests : BunitContext
{
    private readonly IMyAppApi _api = Substitute.For<IMyAppApi>();

    public PageTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        MudBlazor.Extensions.ServiceCollectionExtensions.AddMudExtensions(Services);
        Services.AddSingleton(_api);
        Services.AddSingleton(Substitute.For<ICoworkeeApi>());
        Services.AddSingleton(new CoworkeeClientOptions());
        Services.AddSingleton(Substitute.For<IRealtimeConnection>());
        Services.AddScoped<RealtimeClient>();
        AddAuthorization().SetAuthorized("Ada").SetPolicies(Coworkee.Client.Blazor.Security.PermissionPolicy.For(CatalogPermissions.Manage), Coworkee.Client.Blazor.Security.PermissionPolicy.For(DocumentPermissions.Upload));
        _api.GetDocumentTypesAsync(default).ReturnsForAnyArgs([]);
    }

    [Fact]
    public async Task Brands_are_listed_and_saved()
    {
        _api.GetBrandsAsync(default).ReturnsForAnyArgs([new BrandDto(Guid.CreateVersion7(), "Acme", "Tools", 19, 2)]);
        var page = Render<Brands>();

        page.WaitForAssertion(() => page.Find("[data-brand='Acme']").TextContent.ShouldContain("19 %"));
        page.Find("input[data-testid='brand-name'], [data-testid='brand-name'] input").Change("Globex");
        await page.Find("[data-testid='save-brand']").ClickAsync(new());

        await _api.Received(1).SaveBrandAsync(null, Arg.Is<BrandRequest>(r => r.Name == "Globex"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task A_pdf_is_previewed_with_mudex_file_display()
    {
        var pdf = new DocumentDto(Guid.CreateVersion7(), "Offer", null, true, null, null, "offer.pdf", "application/pdf", 2048, null, DateTimeOffset.UtcNow, false);
        _api.GetDocumentsAsync(default!, default).ReturnsForAnyArgs(new PagedResult<DocumentDto>([pdf], 1, 1, 100));
        var page = Render<Documents>();

        await page.WaitForElement("[data-document='Offer']").ClickAsync(new());

        page.WaitForAssertion(() => page.Find("[data-testid='preview']").InnerHtml.ShouldContain("mud-ex-file-display-pdf"));
        page.Find("[data-document='Offer']").InnerHtml.ShouldNotContain("aria-label=\"Delete\"");
    }
}
