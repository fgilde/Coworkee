using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Coworkee.Contracts;
using Coworkee.Contracts.Identity;
using Coworkee.Testing;
using MyApp.Contracts.Catalog;
using MyApp.Contracts.Documents;

namespace MyApp.Api.Tests;

public sealed class CatalogTests(ApiFixture api) : IAsyncLifetime
{
    private SetupResultDto _setup = null!;

    public async ValueTask InitializeAsync() => _setup = await api.SetupAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private HttpClient Admin => api.As(_setup.AdminUserId, _setup.TenantId);

    [Fact]
    public async Task Brands_and_products_are_managed_searched_and_counted()
    {
        var brand = await (await Admin.PostAsJsonAsync("/api/v1/brands", new BrandRequest("Acme", "Tools", 19), Ct)).Content.ReadFromJsonAsync<BrandDto>(Ct);
        (await Admin.PostAsJsonAsync("/api/v1/brands", new BrandRequest("Acme"), Ct)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await Admin.PostAsJsonAsync("/api/v1/brands", new BrandRequest(""), Ct)).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await Admin.PostAsJsonAsync("/api/v1/products", new ProductRequest("Hammer", brand!.Id, "4001"), Ct)).EnsureSuccessStatusCode();
        (await Admin.PostAsJsonAsync("/api/v1/products", new ProductRequest("Saw", brand.Id, Rate: 12.5m), Ct)).EnsureSuccessStatusCode();
        (await Admin.PostAsJsonAsync("/api/v1/products", new ProductRequest("Ghost", Guid.NewGuid()), Ct)).StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var found = (await Admin.GetFromJsonAsync<PagedResult<ProductDto>>("/api/v1/products?search=4001", Ct))!;
        found.Items.Single().Name.ShouldBe("Hammer");
        found.Items.Single().BrandName.ShouldBe("Acme");
        (await Admin.GetFromJsonAsync<BrandDto[]>("/api/v1/brands", Ct))!.Single().ProductCount.ShouldBe(2);
        (await Admin.DeleteAsync($"/api/v1/brands/{brand.Id}", Ct)).StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var dashboard = (await Admin.GetFromJsonAsync<DashboardDto>("/api/v1/dashboard", Ct))!;
        dashboard.Products.ShouldBe(2);
        dashboard.ProductsPerMonth.Count.ShouldBe(12);
        dashboard.ProductsPerMonth[^1].Count.ShouldBe(2);
    }

    [Fact]
    public async Task Catalog_needs_its_permissions()
    {
        var viewer = await UserWithPermissionsAsync("viewer@acme.test", CatalogPermissions.View);
        (await viewer.GetAsync("/api/v1/brands", Ct)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await viewer.PostAsJsonAsync("/api/v1/brands", new BrandRequest("Nope"), Ct)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        var nobody = await UserWithPermissionsAsync("nobody@acme.test");
        (await nobody.GetAsync("/api/v1/brands", Ct)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Documents_are_uploaded_previewed_and_kept_private()
    {
        var type = await (await Admin.PostAsJsonAsync("/api/v1/document-types", new DocumentTypeRequest("Invoice"), Ct)).Content.ReadFromJsonAsync<DocumentTypeDto>(Ct);
        var uploader = await UserWithPermissionsAsync("uploader@acme.test", DocumentPermissions.Upload);
        var reader = await UserWithPermissionsAsync("reader@acme.test", DocumentPermissions.View);

        var pdf = "%PDF-1.4 test"u8.ToArray();
        (await UploadAsync(uploader, "private.pdf", pdf, "Mine", false, type!.Id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await UploadAsync(uploader, "notes.html", "<script>alert(1)</script>"u8.ToArray(), "Public", true, null)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await UploadAsync(uploader, "tool.exe", [0x4D, 0x5A], "Tool", true, null)).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await UploadAsync(reader, "x.pdf", pdf, "Reader", true, null)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var own = (await uploader.GetFromJsonAsync<PagedResult<DocumentDto>>("/api/v1/documents", Ct))!;
        own.TotalCount.ShouldBe(2);
        var mine = own.Items.Single(d => d.Title == "Mine");
        mine.DocumentTypeName.ShouldBe("Invoice");
        mine.CanEdit.ShouldBeTrue();

        using (var view = await uploader.GetAsync($"/api/v1/documents/{mine.Id}/content", Ct))
        {
            view.Content.Headers.ContentType!.MediaType.ShouldBe("application/pdf");
            view.Content.Headers.ContentDisposition.ShouldBeNull();
            view.Headers.GetValues("Content-Security-Policy").Single().ShouldStartWith("sandbox");
            (await view.Content.ReadAsByteArrayAsync(Ct)).ShouldBe(pdf);
        }

        // html is never shown inline in the app's origin
        var html = own.Items.Single(d => d.Title == "Public");
        using (var download = await reader.GetAsync($"/api/v1/documents/{html.Id}/content", Ct))
        {
            download.Content.Headers.ContentDisposition!.DispositionType.ShouldBe("attachment");
        }

        var visible = (await reader.GetFromJsonAsync<PagedResult<DocumentDto>>("/api/v1/documents", Ct))!;
        visible.Items.Select(d => d.Title).ShouldBe(["Public"]);
        visible.Items.Single().CanEdit.ShouldBeFalse();
        (await reader.GetAsync($"/api/v1/documents/{mine.Id}/content", Ct)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await reader.DeleteAsync($"/api/v1/documents/{html.Id}", Ct)).StatusCode.ShouldBe(HttpStatusCode.NotFound);

        (await Admin.GetFromJsonAsync<PagedResult<DocumentDto>>("/api/v1/documents", Ct))!.TotalCount.ShouldBe(2);
        (await uploader.DeleteAsync($"/api/v1/documents/{mine.Id}", Ct)).StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await Admin.DeleteAsync($"/api/v1/documents/{html.Id}", Ct)).StatusCode.ShouldBe(HttpStatusCode.NoContent, "managers may delete documents of others");
        (await Admin.GetAsync("/api/v1/documents?page=0", Ct)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await uploader.GetAsync($"/api/v1/documents/{mine.Id}", Ct)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private static async Task<HttpResponseMessage> UploadAsync(HttpClient client, string fileName, byte[] content, string title, bool isPublic, Guid? typeId)
    {
        var file = new ByteArrayContent(content);
        file.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        using var form = new MultipartFormDataContent { { file, "file", fileName }, { new StringContent(title), "title" }, { new StringContent(isPublic ? "true" : "false"), "isPublic" } };
        if (typeId is { } id)
        {
            form.Add(new StringContent(id.ToString()), "documentTypeId");
        }

        return await client.PostAsync("/api/v1/documents", form, Ct);
    }

    private async Task<HttpClient> UserWithPermissionsAsync(string email, params string[] permissions)
    {
        var user = (await (await Admin.PostAsJsonAsync("/api/v1/identity/users", new CreateUserRequest(email, "Passw0rd!x", null, null), Ct)).Content.ReadFromJsonAsync<UserDto>(Ct))!;
        var role = (await (await Admin.PostAsJsonAsync("/api/v1/identity/roles", new RoleRequest("Role " + Guid.NewGuid().ToString("N")[..6], null), Ct)).Content.ReadFromJsonAsync<Guid>(Ct))!;
        (await Admin.PutAsJsonAsync($"/api/v1/identity/permissions/grants/Role/{role}", new NameListRequest(permissions), Ct)).EnsureSuccessStatusCode();
        (await Admin.PutAsJsonAsync($"/api/v1/identity/users/{user.Id}/roles", new IdListRequest([role]), Ct)).EnsureSuccessStatusCode();
        return api.As(user.Id, _setup.TenantId);
    }
}
