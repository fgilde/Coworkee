using Coworkee.Localization.Resources;
using MyApp.Contracts.Catalog;

namespace MyApp.Infrastructure;

/// <summary>Texts of the MyApp pages, kept as Localization/{culture}.json in the contracts project.</summary>
internal sealed class MyAppTexts : ILocalizationResourceContributor
{
    public void Define(LocalizationResourceContext context) => context.AddEmbeddedJson(typeof(CatalogPermissions).Assembly);
}
