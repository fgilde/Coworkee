using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.Components.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MyApp.Contracts.Catalog;
using MyApp.Web.Client.Api;

namespace MyApp.Web.Client.Pages.Catalog;

public partial class Brands
{
    private static readonly string[] SearchFields = [nameof(BrandDto.Name), nameof(BrandDto.Description)];
    private CoworkeeDataTable<BrandDto> _table = null!;

    [Inject] private ICatalogApi Api { get; set; } = null!;

    [Inject] private IDialogService Dialogs { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private Task CreateAsync() => EditAsync(null, new AddEditBrandRequest());

    private Task EditAsync(BrandDto brand) =>
        EditAsync(brand.Id, new AddEditBrandRequest { Name = brand.Name, Description = brand.Description, Tax = brand.Tax });

    private async Task EditAsync(Guid? id, AddEditBrandRequest model)
    {
        if (await Dialogs.ShowEditAsync(id is null ? "New brand" : "Edit brand", model) is { } saved)
        {
            await Snackbar.RunAsync(() => Api.SaveBrandAsync(id, saved), "Brand saved");
        }
    }

    private Task DeleteAsync(IReadOnlyCollection<BrandDto> brands) => Snackbar.RunAsync(() => Api.DeleteBrandsAsync([.. brands.Select(b => b.Id)]));
}
