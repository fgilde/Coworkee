using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure;

namespace MyApp.Migrations.Tests;

public sealed class ModelTests
{
    [Fact]
    public void Migrations_cover_the_whole_model_including_auth_tables()
    {
        using var db = new MyAppDbContextDesignFactory().CreateDbContext([]);

        db.Model.GetEntityTypes().ShouldContain(t => t.GetTableName() == "OpenIddictApplications" && t.GetSchema() == "auth");
        db.Database.HasPendingModelChanges().ShouldBeFalse();
    }
}
