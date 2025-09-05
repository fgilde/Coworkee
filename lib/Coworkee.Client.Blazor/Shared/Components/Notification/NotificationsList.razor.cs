using Microsoft.AspNetCore.Components;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Shared.Wrapper;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Threading;


namespace lib.Coworkee.Client.Shared.Components.Notification;

public partial class NotificationsList
{
    private ClaimsPrincipal _currentUser;
    private EditableDataTable<NotificationDto, string> table;
    private NotificationDto _selectedNotification;
    private string _id;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        _currentUser = await _clientAuthenticationManager.CurrentUser();
    }
    [Parameter]
    public string Action { get; set; }

    [Parameter] public string Id { get; set; }
    
    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();
        if (!string.IsNullOrEmpty(Id))
            _selectedNotification = await _api.Notifications_GetByIdAsync(Id);
    }


    private async Task<PaginatedResult<NotificationDto>> Load(int pageNumber, int pageSize, string _searchString, string[] orderings, CancellationToken cancellationToken)
    {
        return await _api.Notifications_GetAllAsync(false, null, pageNumber, pageSize, _searchString, orderings, cancellationToken: cancellationToken);
    }

    private async Task<NotificationDto> FindById(string id, IEnumerable<NotificationDto> loaded)
    {
        return loaded.FirstOrDefault(p => p.Id == id) ?? await _api.Notifications_GetByIdAsync(id);
    }

    private string GetId(NotificationDto notification)
    {
        return notification.Id;
    }

    private async Task<Result> Delete(string[] ids)
    {
        await _api.Notifications_DeleteAsync(ids.ToList());
        return await Result.SuccessAsync("Deleted") as Result;
    }

    private string GetName(NotificationDto arg)
    {
        return arg.Subject;
    }
    
    private async Task OnDelete(NotificationDto arg)
    {
        await table.Reload();
        _selectedNotification = null;
    }

    private string GetRowStyle(NotificationDto notification, int index)
    {
        if (!notification.IsRead)
            return "font-style: italic; text-decoration: underline;";
        return string.Empty;
    }

    private async Task OnMarkRead(NotificationDto arg)
    {
        await table.Reload();
    }
}