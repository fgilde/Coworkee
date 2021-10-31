using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Client.Shared.Dialogs;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using MudBlazor;
using Newtonsoft.Json;
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
        public Func<int, int, string, string[], Task<PaginatedResult<TResult>>> ApiLoadPaged { get; set; }

        [Parameter]
        public Func<Task<Result<List<TResult>>>> ApiLoad { get; set; }

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

        [Parameter]
        public bool? ImmediateSearch { get; set; }

        private string _pageUrl;
        private List<TResult> _flatList = new();
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
            ImmediateSearch ??= ApiLoadPaged == null;
            _pageUrl = _navigationManager.Uri.Split(InitialAction)[0].EnsureEndsWith("/");
            _currentUser = await _clientAuthenticationManager.CurrentUser();
            _canCreate = ApiCreateOrEdit != null && await HasPermission(CreatePermission);
            _canEdit = ApiCreateOrEdit != null && GetId != null && await HasPermission(EditPermission);
            _canDelete = ApiDelete != null && GetId != null && await HasPermission(DeletePermission);
            _canExport = await HasPermission(ExportPermission);
            _canSearch = await HasPermission(SearchPermission);

            if (ApiLoadPaged == null)
                await LoadAllData();

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

            var response = await ApiLoadPaged(pageNumber + 1, pageSize, _searchString, orderings);
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

        private async Task LoadAllData()
        {
            if (ApiLoad != null)
            {
                var response = await ApiLoad();
                if (response.Succeeded)
                {
                    _flatList = response.Data.ToList();
                }
                else
                {
                    foreach (var message in response.Messages)
                    {
                        _snackBar.Add(message, Severity.Error);
                    }
                }
            }
        }

        private void OnSearch(string text)
        {
            _searchString = text;
            if (ApiLoadPaged != null)
                _table.ReloadServerData();
        }

        private async Task Reset(bool keepSearch = false)
        {
            if (ApiLoadPaged != null)
                OnSearch(keepSearch ? _searchString : string.Empty);
            else
                await LoadAllData();
            _selectedItems.Clear();
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
                    FileName = $"{typeof(TResult).Name.ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
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
            bool success = await ApiCreateOrEdit(isDefaultId ? default : await GetById(id, GetLoadedData()));

            if (success)
            {
                await Reset();
            }

            _navigationManager.NavigateTo(_pageUrl);
        }

        private IEnumerable<TResult> GetLoadedData()
        {
            return _pagedData?.Any() == true ? _pagedData : _flatList;
        }

        private async Task<bool> Delete(params TIdType[] ids)
        {
            _navigationManager.NavigateTo($"{_pageUrl}delete/{string.Join(',', ids)}");
            try
            {
                var names = await GetDisplayNamesAsync(ids);
                var value = ids.Length > 1 ? _localizer["Delete these {0} elements"] : _localizer["Delete this element"];
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
                        await Reset(true);
                        await HubConnection.SendAsync(ApplicationConstants.SignalR.SendUpdateDashboard);
                        _snackBar.Add(response.Messages[0], Severity.Success);
                    }
                    else
                    {
                        await Reset(true);
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
                names.Add(Display == null ? id.ToString() : Display(await GetById(id, GetLoadedData())));
            return names;
        }

        private string PropertyValueFor(TResult context, string prop)
        {
            return PropertyValueForAs<string>(context, prop);
        }

        private T PropertyValueForAs<T>(TResult context, string prop)
        {
            return Check.TryCatch<T, Exception>(() => context.ExposeField<object>(prop).MapTo<T>());
        }

        private PropertyInfo PropertyFor(TResult context, string prop)
        {
            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.GetField;
            return Check.TryCatch<PropertyInfo, Exception>(() =>
                context.GetProperties(bindingFlags)?.FirstOrDefault(info => info.Name == prop)
            );
        }

        private async Task DeleteSelected()
        {
            await Delete(_selectedItems.Select(item => GetId(item)).ToArray());
        }

        private bool LocalFilter(TResult item)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            return TableProperties.Select(p => PropertyValueFor(item, p)).Any(s =>
                s.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true);
        }

        #region Inline edit

        private TResult elementBeforeEdit;
        private void InlineEditBackupItem(object element)
        {
            elementBeforeEdit = element.MapTo<TResult>();
        }

        private void InlineEditItemHasBeenCommitted(object element)
        {
            _snackBar.Add("Commit InlineEditItemHasBeenCommitted " + JsonConvert.ToString(element));
        }

        private void InlineEditCommitClick()
        {
            _snackBar.Add("Commit click");
        }

        private void InlineEditResetItemToOriginalValues(object element)
        {
            //element = elementBeforeEdit;
            //((Element)element).Sign = elementBeforeEdit.Sign;
            //((Element)element).Name = elementBeforeEdit.Name;
            //((Element)element).Molar = elementBeforeEdit.Molar;
            //((Element)element).Position = elementBeforeEdit.Position;
        }

        #endregion


        public async ValueTask DisposeAsync()
        {
            if (HubConnection != null)
                await HubConnection.DisposeAsync();
        }

    }
}