using Coworkee.Client.Extensions;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Hubs;
using Coworkee.Domain.Entities.Misc;
using Coworkee.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Coworkee.Application.Hubs.Events;
using Coworkee.Client.Pages.Misc;
using Coworkee.Shared;
using MudBlazor.Extensions;
using MudBlazor.Extensions.Options;

namespace Coworkee.Client.Shared.Components;

public partial class DocumentList
{
    [CascadingParameter] private HubConnection HubConnection { get; set; }

    /**
     * Set to filter documents for specific user
     */
    [Parameter] public string UserId { get; set; }

    [Parameter] public bool CanCreate { get; set; } = true;

    private IEnumerable<DocumentDto> _pagedData;
    private MudTable<DocumentDto> _table;
    private string CurrentUserId { get; set; }
    private int _totalItems;
    private int _currentPage;
    private string _searchString = "";

    private ClaimsPrincipal _currentUser;
    private bool _canCreateDocuments;
    private bool _canEditDocuments;
    private bool _canDeleteDocuments;
    private bool _canSearchDocuments;
    private bool _canViewDocumentExtendedAttributes;
    private bool _loaded;

    protected override async Task OnInitializedAsync()
    {
        _currentUser = await _clientAuthenticationManager.CurrentUser();
        _canCreateDocuments = CanCreate && (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Create)).Succeeded;
        _canEditDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Edit)).Succeeded;
        _canDeleteDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Delete)).Succeeded;
        _canSearchDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Search)).Succeeded;
        _canViewDocumentExtendedAttributes = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentExtendedAttributes.View)).Succeeded;
        _loaded = true;

        var state = await _stateProvider.GetAuthenticationStateAsync();
        var user = state.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            CurrentUserId = user.GetUserId();
        }
        HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
        HubConnection.On<EntitiesUpdated<DocumentDto>>(a =>
        {
            if (a.User.Id != CurrentUserId)
                OnSearch("");
        });
    }

    private async Task<TableData<DocumentDto>> ServerReload(TableState state)
    {
        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            state.Page = 0;
        }
        await LoadData(state.Page, state.PageSize, state);
        return new TableData<DocumentDto> { TotalItems = _totalItems, Items = _pagedData };
    }

    private async Task LoadData(int pageNumber, int pageSize, TableState state)
    {
        var odataFilterQuery = string.IsNullOrEmpty(UserId) ? null : new TransferableExpression<DocumentDto>(d => d.CreatedBy == UserId);
        var response = await _api.Documents_GetAllAsync(pageNumber + 1, pageSize, _searchString, odataFilterQuery: odataFilterQuery);
        if (_errorService.IsSuccessFull(response))
        {
            _totalItems = response.TotalCount;
            _currentPage = response.CurrentPage;
            var data = response.Data;
            var loadedData = data.Where(element =>
            {
                if (string.IsNullOrWhiteSpace(_searchString))
                    return true;
                if (element.Title.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.Description.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (element.DocumentTypeName.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
                    return true;
                return false;
            });
            switch (state.SortLabel)
            {
                case "documentIdField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Id);
                    break;
                case "documentTitleField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Title);
                    break;
                case "documentDescriptionField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.Description);
                    break;
                case "documentDocumentTypeField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, p => p.DocumentTypeName);
                    break;
                case "documentIsPublicField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.IsPublic);
                    break;
                case "documentDateCreatedField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.CreatedOn);
                    break;
                case "documentOwnerField":
                    loadedData = loadedData.OrderByDirection(state.SortDirection, d => d.CreatedBy);
                    break;
            }
            data = loadedData.ToList();
            _pagedData = data;
        }
    }

    private void OnSearch(string text)
    {
        _searchString = text;
        _table.ReloadServerData();
    }

    private async Task InvokeModal(string id = default)
    {
        var parameters = new DialogParameters();
        if (id != default)
        {
            var doc = _pagedData.FirstOrDefault(c => c.Id == id);
            if (doc != null)
            {
                parameters.Add(nameof(AddEditDocumentModal.AddEditDocumentModel), doc);
            }
        }
        var dialog = await _dialogService.ShowWithDefaultOptionsAsync<AddEditDocumentModal>(id == default ? _localizer["Create"] : _localizer["Edit"], parameters);
        var result = await dialog.Result;
        if (!result.Cancelled)
        {
            OnSearch("");
        }
    }

    private async Task Delete(string id)
    {
        var toDelete = new List<string> { id };
        string deleteContent = _localizer["Delete Content {0}"];
        var parameters = new DialogParameters
        {
            {nameof(Dialogs.DeleteConfirmation.Message), string.Format(deleteContent, id)},
            {nameof(Dialogs.DeleteConfirmation.CheckBoxLabel), "Delete file from server"},
            {nameof(Dialogs.DeleteConfirmation.IsChecked), true},
        };
        var options = new DialogOptionsEx { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true, Animations = DialogServiceExtensions.DefaultAnimationNoFullHeight };
        var dialog = await _dialogService.ShowEx<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
        var result = await dialog.Result;
        if (!result.Canceled)
        {
            await _api.Documents_DeleteAsync(toDelete, result.Data as bool?);
            OnSearch("");
            _snackBar.Add(_localizer["Document Deleted"], Severity.Success);
            await HubConnection.SendAsync(nameof(ClientEventHub.UpdateDashboardAsync));
        }
    }

    private void ManageExtendedAttributes(string documentId)
    {
        _navigationManager.NavigateTo($"/extended-attributes/{nameof(Document)}/{documentId}");
    }

    public ValueTask DisposeAsync()
    {
        return HubConnection.TryDisposeAsync();
    }
}