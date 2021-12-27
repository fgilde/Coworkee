using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Options;

namespace CleanArchitectureBase.Client.Extensions;

public static class DialogServiceExtensions
{
    public static async Task<IDialogReference> ShowWithDefaultOptionsAsync<TDialog>(this IDialogService dialogService, string title, DialogParameters parameters) 
        where TDialog : ComponentBase
    {
         //return dialogService.Show<TDialog>(title, parameters, new DialogOptions()
         //{
         //    CloseButton = true,
         //    MaxWidth = MaxWidth.Medium,
         //    FullWidth = true,
         //    DisableBackdropClick = true
         //});
        
        var optionsEx = new DialogOptionsEx
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            DisableBackdropClick = false,
            // Extended
            MaximizeButton = true,
            DragMode = MudDialogDragMode.Simple,
            Position = DialogPosition.CenterRight,
            Animation = AnimationType.SlideIn,
            FullHeight = true,
            DisableSizeMarginY = true,
            DisablePositionMargin = true
        };

        return await dialogService.ShowEx<TDialog>(title, parameters, optionsEx);
    }
}