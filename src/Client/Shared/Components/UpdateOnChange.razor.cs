using System;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Localization;
using MudBlazor;
using MudBlazor.Extensions.Helper;
using MudBlazor.Extensions.Options;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Hubs.Events;
using Coworkee.Client.Extensions;

namespace Coworkee.Client.Shared.Components;

public partial class UpdateOnChange<TDto> : IAsyncDisposable
    //where TDto: IDtoBase
{
    private ClaimsPrincipal _currentUser;
    private string aniStyle => new[] { NotificationStyle == NotificationStyle.FixedOnAppBar ? AnimationType.Slide : AnimationType.Pulse }.GetAnimationCssStyle(TimeSpan.FromMilliseconds(400));
    private bool _notificationOpen;
    private string _notificationMessage;
    private bool singleModel;

    [Inject] private IStringLocalizer<UpdateOnChange<TDto>> _localizer { get; set; }
    [Inject] private ILocalStorageService localStorage { get; set; }
    [Parameter] public NotificationStyle NotificationStyle { get; set; }
    [Parameter] public bool ForceStateChanged { get; set; }
    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public TDto[] Models { get; set; }
    [Parameter] public EventCallback<TDto[]> ModelsChanged { get; set; }

    [Parameter]
    public TDto Model
    {
        get
        {
            singleModel = true;
            return Models != null ? Models.FirstOrDefault() : default;
        }
        set => Models = new[] { value };
    }

    [Parameter] public EventCallback<TDto> ModelChanged { get; set; }
    [Parameter] public ModelChangeBehaviour ChangeBehaviour { get; set; }
    [CascadingParameter] private HubConnection HubConnection { get; set; }

    [Parameter] public Func<Task<TDto>> ModelLoadFn { get; set; }
    [Parameter] public Func<Task<TDto[]>> ModelsLoadFn { get; set; }

    [Parameter]
    public bool? ConfirmUpdate
    {
        get => _confirmUpdate;
        set
        {
            _confirmUpdate = value;
            _ = value == null ? localStorage.RemoveItemAsync(nameof(ConfirmUpdate)) : localStorage.SetItemAsync(nameof(ConfirmUpdate), value);
        }
    }

    [Parameter] public bool UpdateForNotContainingModels { get; set; }
    

    protected Func<Task> UpdateMethod = () => Task.CompletedTask;
    private string eventName;
    private bool? _confirmUpdate;

    protected override async Task OnInitializedAsync()
    {
        if (await localStorage.ContainKeyAsync(nameof(ConfirmUpdate)))
            ConfirmUpdate ??= await localStorage.GetItemAsync<bool>(nameof(ConfirmUpdate));
        _currentUser = (await _stateProvider.GetAuthenticationStateAsync()).User;
        await base.OnInitializedAsync();
        HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
        eventName = HubConnection.On<EntitiesUpdated<TDto>>(async a => await OnEntitiesUpdated(a));
    }

    private async Task OnEntitiesUpdated(EntitiesUpdated<TDto> a)
    {
        var changedFromOtherUser = GetChangesFromOtherUser(a);
        if (changedFromOtherUser?.Any() == true)
        {
            _notificationMessage = GetNotificationMessage(changedFromOtherUser, a);
            if (ConfirmUpdate ?? true)
            {
                UpdateMethod = () => UpdateModels(changedFromOtherUser);
                _notificationOpen = true;
                StateHasChanged();
            }
            else
            {
                await UpdateModels(changedFromOtherUser);
                _snackBar.Add($"{_notificationMessage}{Environment.NewLine}{_localizer["Data has been updated"]}", Severity.Info, config =>
                {
                    config.Action = _localizer["No longer update automatically"];
                    config.ActionColor = Color.Primary;
                    config.OnClick = _ =>
                    {
                        ConfirmUpdate = true;
                        StateHasChanged();
                        return Task.CompletedTask;
                    };
                });
            }

        }
    }

    private string GetNotificationMessage(TDto[] changes, EntitiesUpdated<TDto> changedEvent)
    {
        return _localizer["The user \"{0}\" just changed some of this data", changedEvent.User.FullName];
    }

    private async Task UpdateModels(TDto[] toUpdate)
    {
        
        if (ChangeBehaviour == ModelChangeBehaviour.UpdateData)
        {
            Models ??= Array.Empty<TDto>();
            foreach (var updated in toUpdate)
            {
                var index = Models.ToList().FindIndex(m => m.Equals(updated));
                if (index >= 0)
                {
                    // Handle update
                    Models[index] = updated;
                }
                else
                {
                    // Handle add
                    Models = Models.Concat(new[] {updated}).ToArray();
                }
            }
        }
        else
        {
            if(ModelLoadFn != null)
                Model = await ModelLoadFn();
            if (ModelsLoadFn != null)
                Models = await ModelsLoadFn();
        }
         
        await ModelsChanged.InvokeAsync(Models);
        await ModelChanged.InvokeAsync(Model);
        if(ForceStateChanged)
            StateHasChanged();
    }

    private TDto[] GetChangesFromOtherUser(EntitiesUpdated<TDto> changeEvent)
    {
        if (changeEvent?.Entities == null || changeEvent.Entities.Length == 0 || changeEvent.User.Id == _currentUser.GetUserId())
            return null;

        return UpdateForNotContainingModels
            ? changeEvent.Entities
            : changeEvent.Entities.Where(e => Models?.Any(m => m.Equals(e)) == true).ToArray();
    }

    public async ValueTask DisposeAsync()
    {
        HubConnection.Remove(eventName);
        await HubConnection.StopAsync();
    }

    private async Task OnUpdateClick()
    {
        if (UpdateMethod != null)
            await UpdateMethod();
        CloseNotification();
    }

    private void CloseNotification()
    {
        _notificationOpen = false;
    }

    private Task ReloadAutoCheckChange(bool arg)
    {
        ConfirmUpdate = !arg;
        return Task.CompletedTask;
    }
}

public enum ModelChangeBehaviour
{
    UpdateData,
    Reload
}

public enum NotificationStyle
{
    [Description("change-notification-fixed-appbar")]
    FixedOnAppBar,
    [Description("change-notification-sticky")]
    Sticky
}