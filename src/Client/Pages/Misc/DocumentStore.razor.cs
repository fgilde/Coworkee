using CleanArchitectureBase.Application.Features.Documents.Queries.GetAll;
using CleanArchitectureBase.Client.Extensions;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.Documents.Commands.AddEdit;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;

namespace CleanArchitectureBase.Client.Pages.Misc
{
    public partial class DocumentStore
    {
        private IEnumerable<GetAllDocumentsResponse> _pagedData;
        private MudTable<GetAllDocumentsResponse> _table;
        private string CurrentUserId { get; set; }
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

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
            _canCreateDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Create)).Succeeded;
            _canEditDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Edit)).Succeeded;
            _canDeleteDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Delete)).Succeeded;
            _canSearchDocuments = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Documents.Search)).Succeeded;
            _canViewDocumentExtendedAttributes = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.DocumentExtendedAttributes.View)).Succeeded;

            _loaded = true;

            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            if (user == null) return;
            if (user.Identity?.IsAuthenticated == true)
            {
                CurrentUserId = user.GetUserId();
            }
        }

        private async Task<TableData<GetAllDocumentsResponse>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<GetAllDocumentsResponse> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            var response = await _api.Documents_GetAllAsync(pageNumber + 1, pageSize, _searchString);
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
                    if (element.DocumentType.Contains(_searchString, StringComparison.OrdinalIgnoreCase))
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
                        loadedData = loadedData.OrderByDirection(state.SortDirection, p => p.DocumentType);
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

        private async Task InvokeModal(int id = 0)
        {
            var parameters = new DialogParameters();
            if (id != 0)
            {
                var doc = _pagedData.FirstOrDefault(c => c.Id == id);
                if (doc != null)
                {
                    parameters.Add(nameof(AddEditDocumentModal.AddEditDocumentModel), new AddEditDocumentCommand
                    {
                        Id = doc.Id,
                        Title = doc.Title,
                        Description = doc.Description,
                        URL = doc.URL,
                        IsPublic = doc.IsPublic,
                        DocumentTypeId = doc.DocumentTypeId
                    });
                }
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditDocumentModal>(id == 0 ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                OnSearch("");
            }
        }

        private async Task Delete(int id)
        {
            string deleteContent = _localizer["Delete Content"];
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.Message), string.Format(deleteContent, id)}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var response = await _api.Documents_DeleteAsync(id);
                OnSearch("");
                if (_errorService.IsSuccessFull(response))
                {
                    _snackBar.Add(response.Messages[0], Severity.Success);
                }
            }
        }

        private void ManageExtendedAttributes(int documentId)
        {
            _navigationManager.NavigateTo($"/extended-attributes/{nameof(Document)}/{documentId}");
        }
    }
}