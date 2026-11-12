using Coworkee.Application.Setup;
using MyApp.Catalog.Domain;
using MyApp.Documents.Domain;
using MyApp.Infrastructure;
using Nextended.Core.Attributes;

namespace MyApp.Migrations;

/// <summary>
/// Sample catalog and document types, so a fresh demo shows filled tables, facets and charts: in the run that sets the system up (also a
/// deployment with its first administrator), never again, so what users delete stays deleted.
/// </summary>
[RegisterAs(typeof(ISeedContributor))]
internal sealed class DemoData(MyAppDbContext db) : ISeedContributor
{
    private static readonly (string Name, string Description, decimal Tax, (string Name, string Barcode, decimal Rate)[] Products)[] Catalog =
    [
        ("Nordwind", "Outdoor equipment", 19, [("Trail backpack", "4006381333931", 89.90m), ("Rain jacket", "4006381333948", 149m), ("Trekking poles", "4006381333955", 59.50m)]),
        ("Lumen", "Lights and lamps", 19, [("Desk lamp", "4006381333962", 39.99m), ("Head torch", "4006381333979", 24.90m)]),
        ("Kaffeerösterei Süd", "Coffee and tea", 7, [("Espresso beans 1 kg", "4006381333986", 18.50m), ("Green tea", "4006381333993", 6.90m), ("Filter coffee", "4006381334006", 9.90m)]),
        ("Werkbank", "Tools", 19, [("Cordless drill", "4006381334013", 129m), ("Hammer", "4006381334020", 19.90m), ("Screwdriver set", "4006381334037", 29.90m)]),
    ];

    private static readonly (string Name, string Description)[] DocumentTypes =
    [
        ("Invoice", "Incoming and outgoing invoices"),
        ("Contract", "Signed agreements"),
        ("Data sheet", "Product data sheets"),
    ];

    public int Order => DemoSeed.Order.Data;

    public Task SeedAsync(SeedContext context, CancellationToken cancellationToken)
    {
        if (!context.IsNew)
        {
            return Task.CompletedTask;
        }

        var tenantId = context.TenantId!.Value;
        foreach (var (name, description, tax, products) in Catalog)
        {
            var brand = new Brand { Name = name, Description = description, Tax = tax, TenantId = tenantId };
            db.Add(brand);
            db.AddRange(products.Select(p => new Product { Name = p.Name, Barcode = p.Barcode, Rate = p.Rate, BrandId = brand.Id, TenantId = tenantId }));
        }

        db.AddRange(DocumentTypes.Select(t => new DocumentType { Name = t.Name, Description = t.Description, TenantId = tenantId }));
        return Task.CompletedTask;
    }
}
