using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Coworkee.Contracts;
using Coworkee.Contracts.Identity;
using Coworkee.Testing;
using MyApp.Contracts;
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
    public async Task Brands_and_products_are_managed_queried_with_facets_and_counted()
    {
        var brand = await (await Admin.PostAsJsonAsync("/api/v1/brands", new AddEditBrandRequest { Name = "Acme", Description = "Tools", Tax = 19 }, Ct)).Content.ReadFromJsonAsync<BrandDto>(Ct);
        (await Admin.PostAsJsonAsync("/api/v1/brands", new AddEditBrandRequest { Name = "Acme" }, Ct)).StatusCode.ShouldBe(HttpStatusCode.Conflict);
        (await Admin.PostAsJsonAsync("/api/v1/brands", new AddEditBrandRequest(), Ct)).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await Admin.PostAsJsonAsync("/api/v1/products", new AddEditProductRequest { Name = "Hammer", BrandId = brand!.Id, Barcode = "4001" }, Ct)).EnsureSuccessStatusCode();
        (await Admin.PostAsJsonAsync("/api/v1/products", new AddEditProductRequest { Name = "Saw", BrandId = brand.Id, Rate = 12.5m }, Ct)).EnsureSuccessStatusCode();
        (await Admin.PostAsJsonAsync("/api/v1/products", new AddEditProductRequest { Name = "Ghost", BrandId = Guid.NewGuid() }, Ct)).StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        var found = await ODataAsync<ProductDto>(Admin, "/odata/Products?$filter=Barcode eq '4001'&$expand=Brand&$count=true");
        found.Items.Single().Name.ShouldBe("Hammer");
        found.Items.Single().Brand!.Name.ShouldBe("Acme");
        found.Facets.ShouldContain("Acme");
        (await ODataAsync<ProductDto>(Admin, $"/odata/Products?$filter=BrandId eq {brand.Id}&$count=true")).Count.ShouldBe(2);
        (await Admin.PostAsJsonAsync("/api/v1/brands/delete", new IdsRequest([brand.Id]), Ct)).StatusCode.ShouldBe(HttpStatusCode.Conflict);

        var dashboard = (await Admin.GetFromJsonAsync<DashboardDto>("/api/v1/dashboard", Ct))!;
        dashboard.Products.ShouldBe(2);
        dashboard.Brands.ShouldBe(1);
        dashboard.Users.ShouldBe(1);
        dashboard.ProductsPerMonth.Count.ShouldBe(12);
        dashboard.ProductsPerMonth[^1].Count.ShouldBe(2);
    }

    [Fact]
    public async Task Catalog_needs_its_permissions()
    {
        var viewer = await UserWithPermissionsAsync("viewer@acme.test", CatalogPermissions.Brands.View);
        (await viewer.GetAsync("/odata/Brands", Ct)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await viewer.GetAsync("/odata/Products", Ct)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        (await viewer.PostAsJsonAsync("/api/v1/brands", new AddEditBrandRequest { Name = "Nope" }, Ct)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        var manager = await UserWithPermissionsAsync("manager@acme.test", CatalogPermissions.Dashboards.View);
        var dashboard = (await manager.GetFromJsonAsync<DashboardDto>("/api/v1/dashboard", Ct))!;
        (dashboard.Users, dashboard.Roles).ShouldBe((null, null), "user and role counts need the identity view permissions");
        var nobody = await UserWithPermissionsAsync("nobody@acme.test");
        (await nobody.GetAsync("/odata/Brands", Ct)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Documents_are_uploaded_previewed_and_kept_private()
    {
        var type = await (await Admin.PostAsJsonAsync("/api/v1/document-types", new AddEditDocumentTypeRequest { Name = "Invoice" }, Ct)).Content.ReadFromJsonAsync<DocumentTypeDto>(Ct);
        var uploader = await UserWithPermissionsAsync("uploader@acme.test", DocumentPermissions.Documents.View, DocumentPermissions.Documents.Create, DocumentPermissions.Documents.Delete);
        var reader = await UserWithPermissionsAsync("reader@acme.test", DocumentPermissions.Documents.View, DocumentPermissions.Documents.Delete);

        var pdf = "%PDF-1.4 test"u8.ToArray();
        (await UploadAsync(uploader, "private.pdf", pdf, "Mine", false, type!.Id)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await UploadAsync(uploader, "notes.html", "<script>alert(1)</script>"u8.ToArray(), "Public", true, null)).StatusCode.ShouldBe(HttpStatusCode.OK);
        (await UploadAsync(uploader, "tool.exe", [0x4D, 0x5A], "Tool", true, null)).StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        (await UploadAsync(reader, "x.pdf", pdf, "Reader", true, null)).StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        var own = await ODataAsync<DocumentDto>(uploader, "/odata/Documents?$expand=DocumentType&$count=true");
        own.Count.ShouldBe(2);
        var mine = own.Items.Single(d => d.Title == "Mine");
        mine.DocumentType!.Name.ShouldBe("Invoice");
        (await uploader.GetStringAsync("/odata/Documents", Ct)).ShouldNotContain("BlobKey", Case.Insensitive, "the response filter drops the storage key");

        using (var view = await uploader.GetAsync($"/api/v1/documents/{mine.Id}/content", Ct))
        {
            view.Content.Headers.ContentType!.MediaType.ShouldBe("application/pdf");
            view.Content.Headers.ContentDisposition.ShouldBeNull();
            view.Headers.GetValues("Content-Security-Policy").Single().ShouldStartWith("sandbox");
            (await view.Content.ReadAsByteArrayAsync(Ct)).ShouldBe(pdf);
        }

        // html is shown inline only inside the sandbox, never with scripts in the app's origin
        var html = own.Items.Single(d => d.Title == "Public");
        using (var view = await reader.GetAsync($"/api/v1/documents/{html.Id}/content", Ct))
        {
            view.Headers.GetValues("Content-Security-Policy").Single().ShouldStartWith("sandbox;");
        }
        using (var download = await reader.GetAsync($"/api/v1/documents/{html.Id}/content?download=true", Ct))
        {
            download.Content.Headers.ContentDisposition!.DispositionType.ShouldBe("attachment");
        }

        (await ODataAsync<DocumentDto>(reader, "/odata/Documents")).Items.Select(d => d.Title).ShouldBe(["Public"]);
        (await reader.GetAsync($"/odata/Documents({mine.Id})", Ct)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await reader.GetAsync($"/api/v1/documents/{mine.Id}/content", Ct)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
        (await reader.PostAsJsonAsync("/api/v1/documents/delete", new IdsRequest([html.Id]), Ct)).StatusCode.ShouldBe(HttpStatusCode.NotFound);

        (await ODataAsync<DocumentDto>(Admin, "/odata/Documents?$count=true")).Count.ShouldBe(2);
        (await uploader.PostAsJsonAsync("/api/v1/documents/delete", new IdsRequest([mine.Id]), Ct)).StatusCode.ShouldBe(HttpStatusCode.NoContent);
        (await Admin.PostAsJsonAsync("/api/v1/documents/delete", new IdsRequest([html.Id]), Ct)).StatusCode.ShouldBe(HttpStatusCode.NoContent, "managers may delete documents of others");
        (await uploader.GetAsync($"/api/v1/documents/{mine.Id}", Ct)).StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    private static async Task<(List<T> Items, long? Count, string Facets)> ODataAsync<T>(HttpClient client, string url)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Prefer", "odata.include-annotations=\"cn.facets\"");
        using var response = await client.SendAsync(request, Ct);
        response.EnsureSuccessStatusCode();
        var page = await response.Content.ReadFromJsonAsync<JsonElement>(Ct);
        var items = page.GetProperty("value").Deserialize<List<T>>(JsonSerializerOptions.Web)!;
        long? count = page.TryGetProperty("@odata.count", out var c) ? c.GetInt64() : null;
        var facets = page.EnumerateObject().FirstOrDefault(p => p.Name.EndsWith("facets", StringComparison.OrdinalIgnoreCase)).Value;
        return (items, count, facets.ValueKind == JsonValueKind.Undefined ? string.Empty : facets.ToString());
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
