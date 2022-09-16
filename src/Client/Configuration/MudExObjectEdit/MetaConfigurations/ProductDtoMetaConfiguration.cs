using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MudBlazor;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using CleanArchitectureBase.Application.Common.Models;

namespace CleanArchitectureBase.Client.Configuration.MudExObjectEdit.MetaConfigurations;

public class ProductDtoMetaConfiguration : IObjectMetaConfiguration<ProductDto>
{
    public ProductDtoMetaConfiguration(ILogger<ProductDtoMetaConfiguration> logger)
    {
        logger.LogInformation("Initialize ProductDto Meta");
    }

    public Task ConfigureAsync(ObjectEditMeta<ProductDto> meta)
    {

        return Task.CompletedTask;
    }

}