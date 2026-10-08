using Coworkee.Ai;
using Microsoft.Extensions.DependencyInjection;

namespace MyApp.Api.Tests;

/// <summary>The assistant and MCP clients get the app's requests as tools without registering them one by one.</summary>
public sealed class AiToolTests(ApiFixture api)
{
    [Fact]
    public void Catalog_and_document_requests_become_tools_but_file_streams_do_not()
    {
        var tools = api.Factory.Services.GetRequiredService<AiToolCatalog>().Tools.Select(t => t.Name).ToList();

        tools.ShouldContain("query_data");
        tools.ShouldContain("add_edit_brand");
        tools.ShouldContain("add_edit_product");
        tools.ShouldContain("get_dashboard");
        tools.ShouldContain("update_document");
        tools.ShouldNotContain("open_document");
        tools.ShouldNotContain("upload_document");
    }
}
