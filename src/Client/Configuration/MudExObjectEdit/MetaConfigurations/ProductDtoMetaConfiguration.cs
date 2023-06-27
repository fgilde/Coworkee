using System.Threading.Tasks;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Requests;
using MudBlazor;
using MudBlazor.Extensions.Components.ObjectEdit;
using Nextended.Core;
using MudBlazor.Extensions.Components;

namespace Coworkee.Client.Configuration.MudExObjectEdit.MetaConfigurations;

public class ProductDtoMetaConfiguration : BaseDtoMetaConfiguration<ProductDto, int>
{

    public override async Task ConfigureAsync(ObjectEditMeta<ProductDto> meta)
    {
        meta.Property(p => p.ImageDataURL).Ignore();
        meta.Property(p => p.Brand).RenderData.AddComponentAfter(RenderData.For<MudTextField<BrandDto>>(f =>
        {
            f.Class = "mud-ex-property-validation-component";
            f.For = () => meta.Value.Brand;
            f.ReadOnly = true;
            f.DisableUnderLine = true;
        }));
        meta.Property(p => p.UploadRequest)
            .WithAdditionalAttributes<MudExUploadEdit<UploadRequest>>(a => a.MimeTypes = MimeType.ImageTypes)
            .WithoutLabel()
            .WrapInMudItem(i => i.md = 12)
            .WrapIn<MudCard>(c =>
            {
                c.Outlined = true;
                c.Elevation = 2;
            });
        await base.ConfigureAsync(meta);
    }

}