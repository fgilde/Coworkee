using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Client.Pages.Catalog;
using CleanArchitectureBase.Client.Shared.Dialogs;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using Nextended.Core;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Shared.Components
{
    public partial class EditableDataTable<TResult, TIdType>
    {
        [Parameter] public string InitialAction { get; set; }
        [Parameter] public string InitialIdString { get; set; }
        
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        [Parameter]
        public Func<int, int, string, string[], Task<PaginatedResult<TResult>>> ApiLoad { get; set; }       
        
        [Parameter]
        public Func<TResult, Task<bool>> ApiCreateOrEdit { get; set; }
        
        [Parameter]
        public Func<TIdType[], Task<Result>> ApiDelete { get; set; }

        [Parameter]
        public Func<string, Task<Result<string>>> Export { get; set; }

        [Parameter]
        public Func<TIdType[], Task<Result<string>>> ExportSelected { get; set; }

        [Parameter]
        public Func<TIdType, IEnumerable<TResult>, Task<TResult>> GetById { get; set; }

        [Parameter]
        public Func<TResult, TIdType> GetId { get; set; }

        [Parameter]
        public Func<TResult, string> Display { get; set; }
        
        [Parameter]
        public string[] TableProperties { get; set; }

        [Parameter]
        public string CreatePermission { get; set; }

        [Parameter]
        public string EditPermission { get; set; }

        [Parameter]
        public string DeletePermission { get; set; }

        [Parameter]
        public string ExportPermission { get; set; }

        [Parameter]
        public string SearchPermission { get; set; }

        private string _pageUrl;
        private IEnumerable<TResult> _pagedData;
        private MudTable<TResult> _table;
        private HashSet<TResult> _selectedItems = new();
        private int _totalItems;
        private int _currentPage;
        private string _searchString = "";
   
        private ClaimsPrincipal _currentUser;
        private bool _canCreate;
        private bool _canEdit;
        private bool _canDelete;
        private bool _canExport;
        private bool _canSearch;
        private bool _loaded;
        
        protected override async Task OnInitializedAsync()
        {
            _pageUrl = _navigationManager.Uri.Split(InitialAction)[0].EnsureEndsWith("/");
            _currentUser = await _clientAuthenticationManager.CurrentUser();
            _canCreate = ApiCreateOrEdit != null && await HasPermission(CreatePermission);
            _canEdit = ApiCreateOrEdit != null && GetId != null && await HasPermission(EditPermission);
            _canDelete = ApiDelete != null && GetId != null && await HasPermission(DeletePermission);
            _canExport = await HasPermission(ExportPermission);
            _canSearch = await HasPermission(SearchPermission); 
            
            _loaded = true;
            HubConnection = HubConnection.TryInitialize(_navigationManager);
            if (HubConnection.State == HubConnectionState.Disconnected)
            {
                await HubConnection.StartAsync();
            }

            await ExecuteInitialPageActionAsync();
        }

        private async Task<bool> HasPermission(string permission)
        {
            _currentUser ??= await _clientAuthenticationManager.CurrentUser();
            return string.IsNullOrWhiteSpace(permission) || (await _authorizationService.AuthorizeAsync(_currentUser, permission)).Succeeded;
        }

        private async Task ExecuteInitialPageActionAsync()
        {
            if (InitialAction?.ToLower() == "add")
            {
                await InvokeModal();
            }
            if (InitialAction?.ToLower() == "edit" && !string.IsNullOrWhiteSpace(InitialIdString))
            {
                await InvokeModal(InitialIdString.MapTo<TIdType>());
            }
            if (InitialAction?.ToLower() == "delete" && !string.IsNullOrWhiteSpace(InitialIdString))
            {
                await Delete(InitialIdString.Split(',').MapTo<TIdType[]>());
            }
        }

        private async Task<TableData<TResult>> ServerReload(TableState state)
        {
            if (!string.IsNullOrWhiteSpace(_searchString))
            {
                state.Page = 0;
            }
            await LoadData(state.Page, state.PageSize, state);
            return new TableData<TResult> { TotalItems = _totalItems, Items = _pagedData };
        }

        private async Task LoadData(int pageNumber, int pageSize, TableState state)
        {
            string[] orderings = null;
            if (!string.IsNullOrEmpty(state.SortLabel))
            {
                orderings = state.SortDirection != SortDirection.None ? new[] { $"{state.SortLabel} {state.SortDirection}" } : new[] { $"{state.SortLabel}" };
            }

            var response = await ApiLoad(pageNumber + 1, pageSize, _searchString, orderings);
            if (response.Succeeded)
            {
                _totalItems = response.TotalCount;
                _currentPage = response.CurrentPage;
                _pagedData = response.Data;
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private void OnSearch(string text)
        {
            _searchString = text;
            _table.ReloadServerData();
        }

        private async Task ExportSelectedToExcel()
        {
            var ids = _selectedItems.Select(item => GetId(item)).ToArray();
            var response = await ExportSelected(ids);
            HandleExportResponse(response);

        }
        
        private async Task ExportToExcel()
        {
            var response = await Export(_searchString);
            HandleExportResponse(response);
        }

        private async void HandleExportResponse(Result<string> response)
        {
            if (response.Succeeded)
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(Products).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Products exported"]
                    : _localizer["Filtered Products exported"], Severity.Success);
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        private async Task InvokeModal(TIdType id = default)
        {
            bool isDefaultId = EqualityComparer<TIdType>.Default.Equals(id, default);
            string u = !isDefaultId ? "edit" : "add";
            
            _navigationManager.NavigateTo($"{_pageUrl}{u}/{(!isDefaultId ? id : string.Empty)}");
            bool success = await ApiCreateOrEdit(isDefaultId ? default : await GetById(id, _pagedData));

            if (success)
            {
                OnSearch(string.Empty);
            }
            
            _navigationManager.NavigateTo(_pageUrl);
        }

        private async Task<bool> Delete(params TIdType[] ids)
        {
            _navigationManager.NavigateTo($"{_pageUrl}delete/{string.Join(',', ids)}");
            try
            {
                var names = await GetDisplayNamesAsync(ids);
                var value = ids.Length > 1 ? _localizer["Delete these {0} elements"]: _localizer["Delete this element"];
                var parameters = new DialogParameters
                {
                    {nameof(DeleteConfirmation.Details), names},
                    {nameof(DeleteConfirmation.Message), string.Format(value, ids.Length)}
                };
                var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
                var dialog = _dialogService.Show<DeleteConfirmation>(_localizer["Delete"], parameters, options);
                var result = await dialog.Result;
                if (!result.Cancelled)
                {
                    var response = await ApiDelete(ids);
                    if (response.Succeeded)
                    {
                        OnSearch("");
                        await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                        _snackBar.Add(response.Messages[0], Severity.Success);
                    }
                    else
                    {
                        OnSearch("");
                        foreach (var message in response.Messages)
                        {
                            _snackBar.Add(message, Severity.Error);
                        }
                    }

                    return response.Succeeded;
                }
                return false;
            }
            finally
            {
                _navigationManager.NavigateTo(_pageUrl);
            }
        }

        private async Task<IEnumerable<string>> GetDisplayNamesAsync(TIdType[] ids)
        {
            var names = new List<string>();
            foreach (var id in ids)
            {
                TResult result = await GetById(id, _pagedData);
                names.Add(Display == null ? id.ToString() : Display(result));
            }
            return names;
        }

        private string PropertyValueFor(TResult context, string prop)
        {
            return Check.TryCatch<string, Exception>(() => context.ExposeField<object>(prop)?.ToString());
        }

        private async Task DeleteSelected()
        {
            var ids = _selectedItems.Select(item => GetId(item)).ToArray();
            if (await Delete(ids))
                _selectedItems.Clear();
        }
    }
}