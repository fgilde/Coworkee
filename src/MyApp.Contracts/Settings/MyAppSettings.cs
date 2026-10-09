using Coworkee.Contracts.Settings;

namespace MyApp.Contracts.Settings;

/// <summary>The app settings of MyApp: the built-in sections plus the catalog (Coworkee:Catalog), edited under Administration > System > Settings.</summary>
public sealed class MyAppSettings : CoworkeeAppSettings
{
    public CatalogSettings Catalog { get; set; } = new();
}

public sealed class CatalogSettings
{
    public string Currency { get; set; } = "EUR";

    public decimal DefaultTaxRate { get; set; } = 19;

    public bool ShowPricesWithTax { get; set; } = true;
}
