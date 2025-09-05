using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Requests;
using Coworkee.Client.Shared.Components;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Components;

namespace Coworkee.Client.Configuration.MudExObjectEdit;

internal static class RenderDataManager
{
    public static IServiceCollection AddMudExWithExtendedDefaults(this IServiceCollection services)
    {
        RegisterDefaults();
        return services.AddMudExtensions(c => c.WithoutAutomaticCssLoading());
    }

    private static void RegisterDefaults()
    {
        // Custom Domain
        RenderDataDefaults.RegisterDefault<BrandDto, BrandSelect>(s => s.Value);
        RenderDataDefaults.RegisterDefault<IEnumerable<UploadRequest>, MudExUploadEdit<UploadRequest>>(edit => edit.UploadRequests);
        RenderDataDefaults.RegisterDefault<UploadRequest[], IList<UploadRequest>, MudExUploadEdit<UploadRequest>>(edit => edit.UploadRequests, requests => requests?.ToList() ?? new List<UploadRequest>(), requests => requests?.ToArray() ?? Array.Empty<UploadRequest>());
        RenderDataDefaults.RegisterDefault<IList<UploadRequest>, MudExUploadEdit<UploadRequest>>(edit => edit.UploadRequests);
        RenderDataDefaults.RegisterDefault<UploadRequest, MudExUploadEdit<UploadRequest>>(edit => edit.UploadRequest, edit => edit.AllowMultiple = false);
    }
}