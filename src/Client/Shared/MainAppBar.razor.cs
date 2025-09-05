using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Extensions;
using Coworkee.Client.Extensions;
using Coworkee.Client.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Core;
using MudBlazor.Extensions.Helper;
using MudBlazor.Extensions.Options;

namespace Coworkee.Client.Shared;

public partial class MainAppBar
{
    private AppBarHeader _appBarHeader;
    
    [CascadingParameter] 
    internal MainLayout Layout { get; set; }

    [CascadingParameter]
    internal ClaimsPrincipal CurrentUser { get; set; }


    [CascadingParameter]
    private HubConnection hubConnection { get; set; }

    private void Logout()
    {
        var parameters = new DialogParameters
        {
            {nameof(Dialogs.Logout.ContentText), $"{localizer["Logout Confirmation"]}"},
            {nameof(Dialogs.Logout.ButtonText), $"{localizer["Logout"]}"},
            {nameof(Dialogs.Logout.Color), Color.Error},
            {nameof(Dialogs.Logout.CurrentUserId), CurrentUser.GetUserId()},
            {nameof(Dialogs.Logout.HubConnection), hubConnection}
        };

        var options = new DialogOptionsEx { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, Animations = DialogServiceExtensions.DefaultAnimationNoFullHeight };

        _dialogService.ShowEx<Dialogs.Logout>(localizer["Logout"], parameters, options);
    }

    private async void ShowAbout()
    {
        await _dialogService.ShowWithDefaultOptionsAsync<Dialogs.About>(localizer["about"], null, o =>
        {
            o.MaxWidth = MaxWidth.Small;
            o.BackdropClick = true;
            o.MaximizeButton = false;
        });
    }

    private async Task RightToLeftToggle() => await _clientPreferenceManager.ToggleLayoutDirection();

    private string GetTitle()
    {
        var drawer = Layout?.NavMenuDrawer;
        var menuItem = drawer?.Menu?.FindEntriesForUrl()?.FirstOrDefault();
        var res = drawer?.Menu?.Locale(menuItem?.Parent?.Text ?? menuItem?.Text ?? _appBarHeader?.Title);
        if (string.IsNullOrEmpty(res))
            return _navigationManager.ToBaseRelativePath(_navigationManager.Uri).Split("/").FirstOrDefault()?.ToUpper() ?? "404";
        return res;
    }

    #region Assistant

    private bool pinnedAssistant;

    [JSInvokable]
    public async void PinAssistantClick()
    {
        pinnedAssistant = !pinnedAssistant;
        StateHasChanged();
    }

    private async void OnAssistantClick()
    {
        await _dialogService.ShowEx<Dialogs.AssistantDialog>(localizer["Command"], dialog =>
        {
            
        }, new DialogOptionsEx
        {
            Buttons = new[] { new MudDialogButton(DotNetObjectReference.Create(this as object), nameof(PinAssistantClick)) { Icon = Icons.Material.Filled.PushPin } },
            CloseButton = true,
            AnimationDurationInMs = 250,
            BackdropClick = false,
            DisablePositionMargin = true,
            Resizeable = true,
            DragMode = MudDialogDragMode.Simple,
            Position = DialogPosition.TopCenter,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
            CloseOnEscapeKey = true,
            DialogAppearance = MudExAppearance.FromCss(MudExCss.Classes.Dialog.Glass).WithStyle(b =>
                b.WithMinSize(400)
                    .WithBorderWidth(2)
                    .WithBorderColor(Color.Secondary)
            ),
            Animations = new[] { AnimationType.Perspective3d }
        });
    }
    #endregion

}