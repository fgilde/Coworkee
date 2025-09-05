using System.Threading.Tasks;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Requests;
using MudBlazor;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components;

namespace lib.Coworkee.Client.Configuration.MudExObjectEdit.MetaConfigurations;

public class ProductDtoMetaConfiguration : BaseDtoMetaConfiguration<ProductDto, int>
{

    public override async Task ConfigureAsync(ObjectEditMeta<ProductDto> meta)
    {
        meta.Property(p => p.ImageDataURL).Ignore();               
        meta.Property(p => p.UploadRequest)            
            .WithAdditionalAttributes<MudExUploadEdit<UploadRequest>>(a =>
            {
                a.MimeTypes = new[] { "image/*" };
                a.MimeRestrictionType = RestrictionType.WhiteList;
            })            
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