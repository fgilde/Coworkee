using Coworkee.Client.Blazor.Localization;
using Coworkee.Client.Blazor.Api;
using Coworkee.Client.Blazor.Components.Data;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using MyApp.Contracts.Catalog;
using MyApp.Web.Client.Api;

namespace MyApp.Web.Client.Pages.Catalog;

public partial class Products
{
    [Inject] private CoworkeeLocalizer L { get; set; } = null!;

    private static readonly string[] SearchFields = [nameof(ProductDto.Name), nameof(ProductDto.Barcode), nameof(ProductDto.Description)];
    private CoworkeeDataTable<ProductDto> _table = null!;

    [Inject] private ICatalogApi Api { get; set; } = null!;

    [Inject] private IDialogService Dialogs { get; set; } = null!;

    [Inject] private ISnackbar Snackbar { get; set; } = null!;

    private Task CreateAsync() => EditAsync(null, new AddEditProductRequest());

    private Task EditAsync(ProductDto product) => EditAsync(product.Id, new AddEditProductRequest
    {
        Name = product.Name,
        Barcode = product.Barcode,
        Description = product.Description,
        ImageDataUrl = product.ImageDataUrl,
        Rate = product.Rate,
        BrandId = product.BrandId,
    });

    private async Task EditAsync(Guid? id, AddEditProductRequest model)
    {
        if (await Dialogs.ShowEditAsync(L[id is null ? "New product" : "Edit product"], model, saved => Api.SaveProductAsync(id, saved), Configure))
        {
            Snackbar.Add(L["Product saved"], Severity.Success);
        }
    }

#pragma warning disable BL0005 // MudEx configures the wrapping grid items through these instances
    private void Configure(ObjectEditMeta<AddEditProductRequest> meta)
    {
        meta.Property(p => p.Name).WithLabel(L["Name"]).WithOrder(0);
        meta.Property(p => p.BrandId).WithLabel(L["Brand"]).WithOrder(1)
            .RenderWith<ODataPicker, Guid>(p => p.Value)
            .WithAdditionalAttribute(nameof(ODataPicker.EntitySet), "Brands")
            .WithAdditionalAttribute(nameof(ODataPicker.Required), true);
        meta.Property(p => p.Barcode).WithLabel(L["Barcode"]).WithOrder(2);
        meta.Property(p => p.Rate).WithLabel(L["Rate"]).WithOrder(3);
        meta.Property(p => p.Description).WithLabel(L["Description"]).WithOrder(4).WithAdditionalAttribute("Lines", 3).WrapInMudItem(i => i.md = 12);
        meta.Property(p => p.ImageDataUrl).WithLabel(L["Image"]).WithOrder(5).RenderWith<ImageDataUrlEdit, string?>(p => p.Value).WrapInMudItem(i => i.md = 12);
    }
#pragma warning restore BL0005

    private Task DeleteAsync(IReadOnlyCollection<ProductDto> products) => Snackbar.RunAsync(() => Api.DeleteProductsAsync([.. products.Select(p => p.Id)]));
}
