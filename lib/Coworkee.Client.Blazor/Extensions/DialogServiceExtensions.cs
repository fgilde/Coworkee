using System;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Client.JsInterop;
using lib.Coworkee.Client.JsInterop.Models;
using lib.Coworkee.Client.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Components.ObjectEdit;
using MudBlazor.Extensions.Options;

namespace lib.Coworkee.Client.Extensions;

public static class DialogServiceExtensions
{
    public static AnimationType[] DefaultAnimationNoFullHeight = { AnimationType.FadeIn, AnimationType.FlipX};

    public static async Task<(bool Cancelled, TModel Result)> EditOrCreate<TModel>(this IDialogService dialogService, TModel model, Func<TModel, MudExObjectEditDialog<TModel> , Task<string>> onSave, string titleAdd = "Add", string titleEdit = "Edit" ) where TModel : IDtoBase, new()
    {
        var isNew = model == null || model.IsNew;
        DialogParameters parameters = new()
        {
            { nameof(MudExObjectEditDialog<TModel>.DialogIcon), isNew ? Icons.Material.Filled.Add : Icons.Material.Filled.Edit }
        };
        var title = isNew ? titleAdd : titleEdit;
        return await dialogService.EditObject(model ?? new TModel(), title, onSave, await DefaultDialogOptionsEx(), null, parameters);
    }
    
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
            BackdropClick = false,
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
            return (data.MouseArgs.PageX < data.BrowserDimensions.Width / 2 && !MainLayout.Instance.RightToLeft ) ? DialogPosition.CenterLeft : DialogPosition.CenterRight;
        }
        catch { return DialogPosition.CenterRight; }
    }
}