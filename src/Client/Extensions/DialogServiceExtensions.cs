using System;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Configuration;
using CleanArchitectureBase.Client.Managers.Preferences;
using CleanArchitectureBase.Shared.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Options;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Extensions;

public static class DialogServiceExtensions
{
    public static async Task<IDialogReference> ShowWithDefaultOptionsAsync<TDialog>(this IDialogService dialogService, string title, DialogParameters parameters = null, Action<DialogOptionsEx> options = null) 
        where TDialog : ComponentBase
    {
        var clientPreferenceManager = ServiceAccessor.Get<ClientPreferenceManager>();
        var isRtl = ((ClientPreference) await clientPreferenceManager.GetPreference()).IsRTL;
        var optionsEx = new DialogOptionsEx
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            DisableBackdropClick = true,
            MaximizeButton = true,
            DragMode = MudDialogDragMode.Simple,
            Position = !isRtl ? DialogPosition.CenterRight : DialogPosition.CenterLeft,
            Animation = AnimationType.SlideIn,
            FullHeight = true,
            DisableSizeMarginY = true,
            DisablePositionMargin = true
        };
        options?.Invoke(optionsEx);

        return await dialogService.ShowEx<TDialog>(title, parameters ?? new DialogParameters(), optionsEx);
    }
}