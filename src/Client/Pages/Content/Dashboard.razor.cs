using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Hubs;
using CleanArchitectureBase.Application.Features.Dashboards.Queries.GetData;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.SDK;
using ChartSeries = MudBlazor.ChartSeries;

namespace CleanArchitectureBase.Client.Pages.Content
{
    public partial class Dashboard: IAsyncDisposable
    {
        [Inject] private IApplicationClient Api { get; set; }

        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Parameter] public int ProductCount { get; set; }
        [Parameter] public int BrandCount { get; set; }
        [Parameter] public int DocumentCount { get; set; }
        [Parameter] public int DocumentTypeCount { get; set; }
        [Parameter] public int DocumentExtendedAttributeCount { get; set; }
        [Parameter] public int UserCount { get; set; }
        [Parameter] public int RoleCount { get; set; }

        private readonly string[] _dataEnterBarChartXAxisLabels = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        private readonly List<ChartSeries> _dataEnterBarChartSeries = new();
        private bool _loaded;

        protected override async Task OnInitializedAsync()
        {
            await LoadDataAsync();
            _loaded = true;
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
            HubConnection.On(nameof(IClientEventHub.UpdateDashboard), async () =>
            {
                await LoadDataAsync();
                StateHasChanged();
            });
        }

        private async Task LoadDataAsync()
        {
            var response = await Api.Dashboard_GetDataAsync();
            if (_errorService.IsSuccessFull(response))
                UpdateDataFields(response.Data);
        }

        private void UpdateDataFields(DashboardDataResponse data)
        {
            ProductCount = data.ProductCount;
            BrandCount = data.BrandCount;
            DocumentCount = data.DocumentCount;
            DocumentTypeCount = data.DocumentTypeCount;
            DocumentExtendedAttributeCount = data.DocumentExtendedAttributeCount;
            UserCount = data.UserCount;
            RoleCount = data.RoleCount;
            foreach (var item in data.DataEnterBarChart)
            {
                _dataEnterBarChartSeries
                    .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                _dataEnterBarChartSeries.Add(new ChartSeries {Name = item.Name, Data = item.Data});
            }
        }

        public ValueTask DisposeAsync()
        {
            return HubConnection.TryDisposeAsync();
        }
    }
}