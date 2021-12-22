using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Extensions;

public static class DialogServiceExtensions
{
    public static async Task<IDialogReference> ShowAsync<TDialog>(this IDialogService dialogService, string title, DialogParameters parameters, DialogOptions options) 
        where TDialog : ComponentBase
    {
        // return dialogService.Show<TDialog>(title, parameters, options);
        var optionsEx = options.MapTo<DialogOptionsEx>();
        optionsEx.MaximizeButton = true;
        optionsEx.DragMode = MudDialogDragMode.Simple;
        //optionsEx.Position = DialogPosition.CenterRight;
        //optionsEx.FullWidth = true;
        //optionsEx.MaxWidth = MaxWidth.False;
        return await dialogService.ShowEx<TDialog>(title, parameters, optionsEx);
    }
}