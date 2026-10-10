using Coworkee.Core.Security;
using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure;
using MyApp.Sales.Domain;

namespace MyApp.Migrations;

/// <summary>
/// 250 customers and 100,000 sales orders for the client entity demo, once per database. Postgres generates them in one statement
/// each: the same rows on every machine, dated back from today so the date facets have recent orders.
/// </summary>
public static class DemoSales
{
    public const int Customers = 250;
    public const int Orders = 100_000;

    // one order every 15 minutes: 100,000 orders reach back almost three years
    private const int MinutesApart = 15;

    public static async Task EnsureAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        if (await provider.GetRequiredService<ITenantDirectory>().GetSystemTenantIdAsync(cancellationToken) is not { } tenantId)
        {
            return;
        }

        var db = provider.GetRequiredService<MyAppDbContext>();
        if (await db.Set<Customer>().IgnoreQueryFilters().AnyAsync(c => c.TenantId == tenantId, cancellationToken))
        {
            return;
        }

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await db.Database.ExecuteSqlAsync($"""
            INSERT INTO app."Customers" ("Id", "Name", "City", "TenantId")
            SELECT md5({tenantId}::text || '/customer/' || c)::uuid,
                   (ARRAY['Nordwind','Lumen','Werkbank','Alpenblick','Hanse','Rheinland','Elbtal','Spreewerk','Isartal','Mainufer'])[1 + c % 10] || ' ' ||
                   (ARRAY['Handel','Technik','Logistik','Bau','Medien','Gastro','Textil','Energie','Agrar','Pharma'])[1 + (c / 10) % 10] || ' ' ||
                   (ARRAY['GmbH','AG','KG','e.K.','OHG'])[1 + (c / 100) % 5],
                   (ARRAY['Hamburg','Berlin','München','Köln','Frankfurt','Stuttgart','Düsseldorf','Leipzig','Dresden','Hannover','Bremen','Nürnberg'])[1 + (c * 7) % 12],
                   {tenantId}
            FROM generate_series(0, {Customers - 1}) AS c
            """, cancellationToken);
        await db.Database.ExecuteSqlAsync($"""
            INSERT INTO app."SalesOrders" ("Id", "Number", "CustomerId", "Status", "Channel", "Region", "OrderDate", "Items", "Total", "TenantId", "CreatedAt")
            SELECT md5({tenantId}::text || '/order/' || g)::uuid,
                   'SO-' || lpad(g::text, 7, '0'),
                   md5({tenantId}::text || '/customer/' || (g * 7919) % {Customers})::uuid,
                   CASE WHEN g % 37 = 0 THEN 4 WHEN age < 192 THEN 0 WHEN age < 480 THEN 1 WHEN age < 960 THEN 2 ELSE 3 END,
                   (g * 31 + g / 7) % 4,
                   (ARRAY['Nord','Süd','Ost','West','Mitte'])[1 + ((g * 7919) % {Customers}) % 5],
                   date,
                   1 + (g * 13) % 12,
                   round((1 + (g * 13) % 12) * (4.90 + ((g::bigint * 104729) % 25000) / 100.0), 2),
                   {tenantId},
                   date
            FROM generate_series(1, {Orders}) AS g,
                 LATERAL (SELECT {Orders} - g AS age) AS a,
                 LATERAL (SELECT now() - (a.age * {MinutesApart}) * interval '1 minute' AS date) AS d
            """, cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
