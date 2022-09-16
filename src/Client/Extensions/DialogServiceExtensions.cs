using System;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.JsInterop;
using CleanArchitectureBase.Client.JsInterop.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Options;

namespace CleanArchitectureBase.Client.Extensions;

public static class DialogServiceExtensions
{
    public static AnimationType[] DefaultAnimationNoFullHeight = { AnimationType.FadeIn, AnimationType.FlipX};

    public static async Task<IDialogReference> ShowWithDefaultOptionsAsync<TDialog>(this IDialogService dialogService, string title, DialogParameters parameters = null, Action<DialogOptionsEx> options = null)
        where TDialog : ComponentBase
    {
        var optionsEx = await DefaultDialogOptionsEx();
        options?.Invoke(optionsEx);
        return await dialogService.ShowEx<TDialog>(title, parameters ?? new DialogParameters(), optionsEx);
    }

    internal static async Task<DialogOptionsEx> DefaultDialogOptionsEx()
    {
        var optionsEx = new DialogOptionsEx
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            DisableBackdropClick = true,
            MaximizeButton = true,
            DragMode = MudDialogDragMode.Simple,
            Position = await PositionBasedOnMouse(),
            Animations = new[] { AnimationType.SlideIn },
            FullHeight = true,
            DisableSizeMarginY = true,
            DisablePositionMargin = true
        };
        return optionsEx;
    }

    internal static async Task<DialogPosition> PositionBasedOnMouse()
    {
        try
        {
            var data = await ServiceAccessor.Get<IJSRuntime>().InvokeAsync<JsAppData>(JsNamespace.Get("getJsAppData"));
            return data.MouseArgs.PageX < data.BrowserDimensions.Width / 2 ? DialogPosition.CenterLeft : DialogPosition.CenterRight;
        }
        catch { return DialogPosition.CenterRight; }
    }
}