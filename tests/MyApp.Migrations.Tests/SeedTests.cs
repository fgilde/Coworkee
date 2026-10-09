using System.Diagnostics;
using Coworkee.Testing;
using Npgsql;

namespace MyApp.Migrations.Tests;

public sealed class SeedTests
{
    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Every_run_brings_back_the_seeded_roles_without_touching_users_or_the_demo_data()
    {
        await using var postgres = new PostgresFixture();
        await postgres.InitializeAsync();
        (await RunAsync(postgres.ConnectionString)).ShouldBe(0);
        var hashes = await ScalarAsync<string>(postgres.ConnectionString, "SELECT string_agg(\"PasswordHash\", ',' ORDER BY \"Email\") FROM cw.\"Users\"");
        var products = await ScalarAsync<long>(postgres.ConnectionString, "SELECT count(*) FROM app.\"Products\"");

        // a database seeded before the Customer role existed
        await ScalarAsync<int>(postgres.ConnectionString, """
            DELETE FROM cw."PermissionGrants" WHERE "ProviderKey" IN (SELECT "Id" FROM cw."Roles" WHERE "Name" = 'Customer');
            DELETE FROM cw."Roles" WHERE "Name" = 'Customer';
            UPDATE cw."Roles" SET "Description" = 'changed by an admin' WHERE "Name" = 'Brand Manager';
            SELECT 0
            """);
        (await RunAsync(postgres.ConnectionString)).ShouldBe(0);

        (await ScalarAsync<bool>(postgres.ConnectionString, "SELECT \"SelectableForRegistration\" FROM cw.\"Roles\" WHERE \"Name\" = 'Customer'")).ShouldBeTrue();
        (await ScalarAsync<long>(postgres.ConnectionString,
            "SELECT count(*) FROM cw.\"PermissionGrants\" g JOIN cw.\"Roles\" r ON r.\"Id\" = g.\"ProviderKey\" WHERE r.\"Name\" = 'Customer'")).ShouldBe(3);
        (await ScalarAsync<string>(postgres.ConnectionString, "SELECT \"Description\" FROM cw.\"Roles\" WHERE \"Name\" = 'Brand Manager'")).ShouldBe("changed by an admin");
        (await ScalarAsync<string>(postgres.ConnectionString, "SELECT string_agg(\"PasswordHash\", ',' ORDER BY \"Email\") FROM cw.\"Users\"")).ShouldBe(hashes);
        (await ScalarAsync<long>(postgres.ConnectionString, "SELECT count(*) FROM app.\"Products\"")).ShouldBe(products);
    }

    private static async Task<int> RunAsync(string connectionString)
    {
        var start = new ProcessStartInfo("dotnet", Path.Combine(AppContext.BaseDirectory, "MyApp.Migrations.dll"))
        {
            Environment = { ["DOTNET_ENVIRONMENT"] = "Production", ["ConnectionStrings__myapp"] = connectionString },
        };
        using var process = Process.Start(start)!;
        await process.WaitForExitAsync(Ct).WaitAsync(TimeSpan.FromMinutes(3), Ct);
        return process.ExitCode;
    }

    private static async Task<T> ScalarAsync<T>(string connectionString, string sql)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(Ct);
        await using var command = new NpgsqlCommand(sql, connection);
        return (T)(await command.ExecuteScalarAsync(Ct))!;
    }
}
