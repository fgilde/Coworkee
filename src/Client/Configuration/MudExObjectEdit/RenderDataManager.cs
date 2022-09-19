using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Components.ObjectEdit.Options;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Requests;
using CleanArchitectureBase.Client.Shared.Components;
using MudBlazor.Extensions;

namespace CleanArchitectureBase.Client.Configuration.MudExObjectEdit;

internal static class RenderDataManager
{
    public static IServiceCollection AddMudExWithExtendedDefaults(this IServiceCollection services)
    {
        RegisterDefaults();
        return services.AddMudExtensions();
    }

    private static void RegisterDefaults()
    {
        // Custom Domain
        RenderDataDefaults.RegisterDefault<BrandDto, BrandSelect>(s => s.Value);
        //RenderDataDefaults.RegisterDefault<ICollection<SpecializationDto>, IEnumerable<SpecializationDto>, SpecializationsSelect>(s => s.Selected);
        //RenderDataDefaults.RegisterDefault<IEnumerable<SpecializationDto>, SpecializationsSelect>(s => s.Selected);
        RenderDataDefaults.RegisterDefault<IEnumerable<UploadRequest>, UploadRequestEdit>(edit => edit.UploadRequests);
        RenderDataDefaults.RegisterDefault<UploadRequest[], IList<UploadRequest>, UploadRequestEdit>(edit => edit.UploadRequests, requests => requests?.ToList() ?? new List<UploadRequest>(), requests => requests?.ToArray() ?? Array.Empty<UploadRequest>());
        RenderDataDefaults.RegisterDefault<IList<UploadRequest>, UploadRequestEdit>(edit => edit.UploadRequests);
        RenderDataDefaults.RegisterDefault<UploadRequest, UploadRequestEdit>(edit => edit.UploadRequest, edit => edit.AllowMultiple = false);
    }
}