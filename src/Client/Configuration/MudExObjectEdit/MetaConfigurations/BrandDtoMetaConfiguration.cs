using System.Threading.Tasks;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using lib.Coworkee.Application.Common.Models;

namespace Coworkee.Client.Configuration.MudExObjectEdit.MetaConfigurations;

public class BrandDtoMetaConfiguration : BaseDtoMetaConfiguration<BrandDto, int>
{
    public override async Task ConfigureAsync(ObjectEditMeta<BrandDto> meta)
    {
        await base.ConfigureAsync(meta);
    }

}