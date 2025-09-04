using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Coworkee.Application.Common.Extensions;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Models.Identity;
using Coworkee.Application.Contracts.Enums;
using Coworkee.Application.Hubs.Events;
using Coworkee.Client.Extensions;
using Coworkee.Shared.Constants.Permission;
using Coworkee.Shared.Constants.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Coworkee.Client.Pages.Utilities
{
    public partial class AuditTrails : IAsyncDisposable
    {
        [CascadingParameter] private HubConnection HubConnection { get; set; }

        public List<RelatedAuditTrail> Trails = new();

        private string _searchString = "";
        private bool _searchInOldValues;
        private bool _searchInNewValues;
        private MudDateRangePicker _dateRangePicker;
        private DateRange _dateRange;

        private ClaimsPrincipal _currentUser;
        private bool _canExportAuditTrails;
        private bool _canSearchAuditTrails;
        private bool _loaded;
        private IList<string> userFilter;
        private IList<UserResponse> currentUserFilter;
        private IList<UserResponse> allUsers;

        private bool Search(AuditDto response)
        {
            bool result = string.IsNullOrWhiteSpace(_searchString);

            // check Search String
            if (!result)
            {
                if (response.TableName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                {
                    result = true;
                }
                if (_searchInOldValues &&
                    response.OldValues?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                {
                    result = true;
                }
                if (_searchInNewValues &&
                    response.NewValues?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
                {
                    result = true;
                }
            }

            // check Date Range
            if (_dateRange?.Start == null && _dateRange?.End == null) return result;
            if (_dateRange?.Start != null && response.DateTime < _dateRange.Start)
            {
                result = false;
            }
            if (_dateRange?.End != null && response.DateTime > _dateRange.End + new TimeSpan(0,11, 59, 59, 999))
            {
                result = false;
            }

            return result;
        }

        protected override async Task OnInitializedAsync()
        {
            _currentUser = await _clientAuthenticationManager.CurrentUser();
            _canExportAuditTrails = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AuditTrails.Export)).Succeeded;
            _canSearchAuditTrails = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.AuditTrails.Search)).Succeeded;

            if (_currentUser.IsInRole(RoleConstants.AdministratorRole)) // Current user is admin and can filter for users
            {
                userFilter = new List<string> {_currentUser.GetUserId()};
                if ((await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.View)).Succeeded)
                {
                    allUsers = (await _api.User_GetAllAsync()).Data;
                    currentUserFilter = new List<UserResponse> {allUsers.First(r => r.Id == _currentUser.GetUserId())};
                }
                else
                {
                    var currentAsResponse = (await _api.User_GetByIdAsync(_currentUser.GetUserId())).Data;
                    currentUserFilter = new List<UserResponse> {currentAsResponse};
                }
            }

            await GetDataAsync();
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);

            HubConnection.On<EntitiesUpdated>(async (a) =>
            {
                await GetDataAsync();
                StateHasChanged();
            });

            _loaded = true;
        }

        private async Task GetDataAsync()
        {
            var response = await _api.Audits_GetUserTrailsAsync(userFilter);

            Trails = response
                .Select(x => new RelatedAuditTrail
                {
                    AffectedColumns = x.AffectedColumns,
                    DateTime = x.DateTime,
                    Id = x.Id,
                    NewValues = x.NewValues,
                    OldValues = x.OldValues,
                    PrimaryKey = x.PrimaryKey,
                    TableName = x.TableName,
                    Type = x.Type,
                    UserId = x.UserId,
                    LocalTime = x.DateTime.AsClientLocalTime()
                }).ToList();
            
        }

        private void ShowBtnPress(RelatedAuditTrail trail)
        {
            trail.ShowDetails = !trail.ShowDetails;
        }

        private async Task ExportToExcelAsync()
        {
            var res = await _api.Audits_ExportAsync(ExportServiceType.Excel, userFilter, _searchString, _searchInOldValues, _searchInNewValues);
            await res.ForceDownloadAsync(_jsRuntime);
        }

        private async void UserChanged(IEnumerable<UserResponse> filteredUsers)
        {
            currentUserFilter = new List<UserResponse>(filteredUsers);
            userFilter = new List<string>(currentUserFilter.Select(r => r.Id));
            await GetDataAsync();
            StateHasChanged();
        }
        
        public class RelatedAuditTrail : AuditDto
        {
            public bool ShowDetails { get; set; } = false;
            public DateTime LocalTime { get; set; }
        }

        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }

    }
}