using System.Text.Json;
using System.Text.Json.Nodes;

namespace MyApp.Api.Tests;

/// <summary>The SDKs are generated from sdk/openapi.json; this keeps the file equal to what the API serves.</summary>
public sealed class OpenApiTests(ApiFixture api)
{
    [Fact]
    public async Task The_sdk_snapshot_matches_the_api()
    {
        var live = await api.Factory.CreateClient().GetStringAsync("/openapi/v1.json", TestContext.Current.CancellationToken);
        var pretty = JsonNode.Parse(live)!.ToJsonString(new JsonSerializerOptions { WriteIndented = true, NewLine = "\n" }) + "\n";
        var path = Path.Combine(RepositoryRoot(), "sdk", "openapi.json");
        if (Environment.GetEnvironmentVariable("MYAPP_UPDATE_OPENAPI") == "1")
        {
            await File.WriteAllTextAsync(path, pretty, TestContext.Current.CancellationToken);
        }

        (await File.ReadAllTextAsync(path, TestContext.Current.CancellationToken)).ReplaceLineEndings("\n")
            .ShouldBe(pretty, "The API changed: run this test with MYAPP_UPDATE_OPENAPI=1 and regenerate the SDKs (see sdk/README.md).");
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MyApp.slnx")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName ?? throw new InvalidOperationException("MyApp.slnx not found above the test output.");
    }
}
