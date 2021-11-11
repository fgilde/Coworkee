using CleanArchitectureBase.Application.Responses.Audit;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CleanArchitectureBase.SDK;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace CleanArchitectureBase.Client.Pages.Utilities
{
    public partial class AuditTrails
    {
        [Inject] private IBlazorHeroClient Api { get; set; }

        public List<RelatedAuditTrail> Trails = new();

        private RelatedAuditTrail _trail = new();
        private string _searchString = "";
        private bool _dense = true;
        private bool _striped = true;
        private bool _bordered = false;
        private bool _searchInOldValues = false;
        private bool _searchInNewValues = false;
        private MudDateRangePicker _dateRangePicker;
        private DateRange _dateRange;

        private ClaimsPrincipal _currentUser;
        private bool _canExportAuditTrails;
        private bool _canSearchAuditTrails;
        private bool _loaded;

        private bool Search(AuditResponse response)
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

            await GetDataAsync();
            _loaded = true;
        }

        private async Task GetDataAsync()
        {
            var response = await Api.Audits_GetUserTrailsAsync();
            if (_errorService.IsSuccessFull(response))
            {
                Trails = response.Data
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
                        LocalTime = DateTime.SpecifyKind(x.DateTime, DateTimeKind.Utc).ToLocalTime()
                    }).ToList();
            }
        }

        private void ShowBtnPress(int id)
        {
            _trail = Trails.First(f => f.Id == id);
            foreach (var trial in Trails.Where(a => a.Id != id))
            {
                trial.ShowDetails = false;
            }
            _trail.ShowDetails = !_trail.ShowDetails;
        }

        private async Task ExportToExcelAsync()
        {
            var response = await Api.Audits_ExportExcelAsync(_searchString, _searchInOldValues, _searchInNewValues);
            if (_errorService.IsSuccessFull(response))
            {
                await _jsRuntime.InvokeVoidAsync("Download", new
                {
                    ByteArray = response.Data,
                    FileName = $"{nameof(AuditTrails).ToLower()}_{DateTime.Now:ddMMyyyyHHmmss}.xlsx",
                    MimeType = ApplicationConstants.MimeTypes.OpenXml
                });
                _snackBar.Add(string.IsNullOrWhiteSpace(_searchString)
                    ? _localizer["Audit Trails exported"]
                    : _localizer["Filtered Audit Trails exported"], Severity.Success);
            }
        }

        public class RelatedAuditTrail : AuditResponse
        {
            public bool ShowDetails { get; set; } = false;
            public DateTime LocalTime { get; set; }
        }
    }
}