using System.Net;
using Coworkee.Client;
using Coworkee.Contracts.Identity;
using MyApp.Client;
using MyApp.Contracts.Catalog;
using MyApp.Contracts.Documents;

namespace MyApp.Api.Tests;

/// <summary>The .NET SDK against the real API: what an integration does first.</summary>
public sealed class ClientSdkTests(ApiFixture api) : IAsyncLifetime
{
    private SetupResultDto _setup = null!;

    public async ValueTask InitializeAsync() => _setup = await api.SetupAsync();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Maintains_the_catalog_and_uploads_documents()
    {
        var client = new MyAppClient(api.As(_setup.AdminUserId, _setup.TenantId));

        var brand = await client.SaveBrandAsync(null, new AddEditBrandRequest { Name = "Acme", Tax = 19 }, Ct);
        await client.SaveProductAsync(null, new AddEditProductRequest { Name = "Hammer", BrandId = brand.Id, Rate = 9.5m }, Ct);
        var products = await client.GetProductsAsync("Rate gt 5", cancellationToken: Ct);
        (products.Count, products.Items.Single().Brand!.Name).ShouldBe((1L, "Acme"));

        var type = await client.SaveDocumentTypeAsync(null, new AddEditDocumentTypeRequest { Name = "Invoice" }, Ct);
        using var file = new MemoryStream("hello sdk"u8.ToArray());
        var document = await client.UploadDocumentAsync(new UpdateDocumentRequest { Title = "First invoice", DocumentTypeId = type.Id }, "invoice.txt", file, Ct);
        await using (var content = await client.OpenDocumentAsync(document.Id, Ct))
        {
            (await new StreamReader(content).ReadToEndAsync(Ct)).ShouldBe("hello sdk");
        }

        var failure = await Should.ThrowAsync<CoworkeeApiException>(() => client.SaveBrandAsync(null, new AddEditBrandRequest(), Ct));
        (failure.Status, failure.Errors.Keys.Single()).ShouldBe((HttpStatusCode.BadRequest, "Brand.Name"));
    }
}
