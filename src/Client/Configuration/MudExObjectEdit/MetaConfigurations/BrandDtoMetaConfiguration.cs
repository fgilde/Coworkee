using System.Threading.Tasks;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using CleanArchitectureBase.Application.Common.Models;

namespace CleanArchitectureBase.Client.Configuration.MudExObjectEdit.MetaConfigurations;

public class BrandDtoMetaConfiguration : BaseDtoMetaConfiguration<BrandDto, int>
{
    public override async Task ConfigureAsync(ObjectEditMeta<BrandDto> meta)
    {
        await base.ConfigureAsync(meta);
    }

}