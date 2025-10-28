using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Coworkee.Infrastructure.Contexts
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Pfad zum Startup-Projekt ermitteln (falls Migrations aus /Infrastructure ausgeführt werden)
            var basePath = Directory.GetCurrentDirectory();
            // OPTIONAL: Wenn dein Startup-Projekt z.B. Coworkee.Api ist, kannst du hier gezielt hochnavigieren:
            // while (!File.Exists(Path.Combine(basePath, "appsettings.json")) && Directory.GetParent(basePath) is { } p) basePath = p.FullName;

            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                config.GetConnectionString("DefaultConnection")
                ?? "Server=localhost;Database=Coworkee;Trusted_Connection=True;TrustServerCertificate=True";

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
            });

            return new ApplicationDbContext(optionsBuilder.Options, null, null);
        }
    }
}