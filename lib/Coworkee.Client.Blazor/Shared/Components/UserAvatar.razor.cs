using System.Linq;
using lib.Coworkee.Application.Common.Models.Identity;
using lib.Coworkee.Application.Hubs.Events;
using lib.Coworkee.Client.Extensions;
using lib.Coworkee.Client.Utils;
using lib.Coworkee.Shared.Constants.Storage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Security.Claims;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Extensions;
using MudBlazor.Extensions.Helper;
using System;

namespace lib.Coworkee.Client.Shared.Components;

public partial class UserAvatar
{
    [CascadingParameter] private HubConnection HubConnection { get; set; }
    [Parameter] public string ImageDataUrl { get; set; }
    [Parameter] public bool LinkToProfile { get; set; } = true;
    [Parameter] public ClaimsPrincipal User { get; set; }
    [Parameter] public UserResponse UserData { get; set; }
    [Parameter] public string UserId { get; set; }
    [Parameter] public int Height { get; set; } = 50;
    [Parameter] public int Width { get; set; } = 50;
    [Parameter] public bool? IsUserOnline { get; set; }
    [Parameter] public bool ShowUserOnlineStatus { get; set; } = true;
    [Parameter] public NoImageHandling NoImageHandling { get; set; } = NoImageHandling.ShowInitials;

    private bool _handleOnlineStatus;
    private string Initials { get; set; }

    private ClaimsPrincipal _currentUser;

    private string StyleStr()
    {
        string color = GetColor();
        return MudExStyleBuilder.Default
            .With("transition-property", "background-color")
            .With("transition-duration", "1s")
            .WithHeight(Height)
            .WithWidth(Width)
            .WithBackgroundColor(color)
            .Style;        
    }

    string GetColor()
    {
        var id = UserData?.Id ?? _currentUser?.GetUserId();
        return string.IsNullOrWhiteSpace(id) ? "transparent" : ColorUtils.Random(id);
    }

    protected override async Task OnParametersSetAsync()
    {
        _handleOnlineStatus = !IsUserOnline.HasValue;
        await Load();

        HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);

        HubConnection.On<UserOnlineStatusChanged>(a =>
        {
            var actualDisplayedUserId = UserId ?? UserData?.Id ?? User?.GetUserId();
            if (a.User.Id == actualDisplayedUserId)
            {
                UpdateOnlineStatusIf(a.User.IsOnline);
                StateHasChanged();
            }
        });

        HubConnection.On<UserProfileChanged>(a =>
        {
            var actualDisplayedUserId = UserId ?? UserData?.Id ?? User?.GetUserId();
            if (a.User.Id == actualDisplayedUserId)
            {
                ImageDataUrl = string.Empty;
                UpdateOnlineStatusIf(a.User.IsOnline);
                Task.Delay(50).ContinueWith(t =>
                {
                    UserData = a.User;
                    SetImageDataUrl(UserData.ProfilePictureDataUrl);
                });
            }
        });
        _localStorage.Changed += async (sender, args) =>
        {
            if (args.Key == StorageConstants.Local.AuthToken && (User != null || _currentUser != null))
            {
                User = _currentUser = (await _stateProvider.GetAuthenticationStateAsync()).User;
                StateHasChanged();
            }
            else if (args.Key == StorageConstants.Local.UserImageURL && (User != null || _currentUser != null))
            {
                SetImageDataUrl(args.NewValue as string);
            }
        };

    }

    protected override async Task OnInitializedAsync()
    {
        _currentUser = (await _stateProvider.GetAuthenticationStateAsync()).User;
    }

    private async Task Load()
    {
        if (string.IsNullOrWhiteSpace(UserId) && UserData == null)
            await LoadCurrentUserData();
        else
            await LoadOtherUserData();        
        var initial = Initials;
        Console.WriteLine(initial);
        await InvokeAsync(StateHasChanged);
    }

    private async Task LoadCurrentUserData()
    {
        var user = User ?? _currentUser;
        if (user?.Identity?.IsAuthenticated == true && !user.IsGuest())
        {
            Initials = user.GetInitials();
            var imageResponse = await _api.Account_GetProfilePictureAsync(user.GetUserId());
            if (imageResponse.Succeeded)
            {
                SetImageDataUrl(imageResponse.Data);
            }
        }
    }

    protected async Task LoadOtherUserData()
    {
        if ((UserData == null || UserData.Id != UserId) && !string.IsNullOrWhiteSpace(UserId))
        {
            var result = await _api.User_GetByIdAsync(UserId);
            if (result.Succeeded)
                UserData = result.Data;

        }
        if (UserData != null)
        {
            Initials = new(new[] { UserData.FirstName.FirstOrDefault(), UserData.LastName.FirstOrDefault() });
            UpdateOnlineStatusIf();
            var data = await _api.Account_GetProfilePictureAsync(UserData.Id);
            if (data.Succeeded)
            {
                SetImageDataUrl(data.Data);
            }
        }
    }

    private void UpdateOnlineStatusIf() => UpdateOnlineStatusIf(UserData.IsOnline);
    private void UpdateOnlineStatusIf(bool isOnline)
    {
        if (_handleOnlineStatus)
            IsUserOnline = isOnline;
    }

    private string TargetProfileUrl()
    {
        if (User != null || UserData?.Id == _currentUser?.GetUserId())
            return "account";
        if (UserData != null)
            return $"user-profile/{UserData.Id}";
        return string.Empty;
    }

    private bool CanNavigateToProfile()
    {
        var url = TargetProfileUrl();
        var currentUrl = _navigationManager.ToBaseRelativePath(_navigationManager.Uri);
        return LinkToProfile && !string.IsNullOrWhiteSpace(url) && currentUrl != url;
    }

    private void GotoAccountOrProfile()
    {
        if (CanNavigateToProfile())
            _navigationManager.NavigateTo(TargetProfileUrl());
    }

    public ValueTask DisposeAsync()
    {
        return HubConnection.TryDisposeAsync();
    }

    public void SetImageDataUrl(string uri, bool stateChange = true)
    {
        ImageDataUrl = _navigationManager.ToAbsoluteServerUri(uri);
        if (stateChange)
            StateHasChanged();
    }

    private string ToolTipText()
    {
        return $"{User?.GetFullName() ?? UserData.FullName} ({(IsUserOnline ?? false ? _localizer["Online"] : _localizer["Offline"])})";
    }

}

public enum NoImageHandling
{    
    ShowGravatar,
    ShowInitials
}