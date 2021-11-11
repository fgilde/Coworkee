using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using MudBlazor;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.SDK;
using CleanArchitectureBase.Shared.Constants.Application;

namespace CleanArchitectureBase.Client.Pages.Content
{
    public partial class Dashboard
    {
        [Inject] private IBlazorHeroClient Api { get; set; }

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
            HubConnection = new HubConnectionBuilder()
            .WithUrl(_navigationManager.ToAbsoluteUri(ApplicationConstants.SignalR.EventHubUrl))
            .Build();
            HubConnection.On(ApplicationConstants.SignalR.ReceiveUpdateDashboard, async () =>
            {
                await LoadDataAsync();
                StateHasChanged();
            });
            await HubConnection.StartAsync();
        }

        private async Task LoadDataAsync()
        {
            var response = await Api.Dashboard_GetDataAsync();
            if (_errorService.IsSuccessFull(response))
            {
                ProductCount = response.Data.ProductCount;
                BrandCount = response.Data.BrandCount;
                DocumentCount = response.Data.DocumentCount;
                DocumentTypeCount = response.Data.DocumentTypeCount;
                DocumentExtendedAttributeCount = response.Data.DocumentExtendedAttributeCount;
                UserCount = response.Data.UserCount;
                RoleCount = response.Data.RoleCount;
                foreach (var item in response.Data.DataEnterBarChart)
                {
                    _dataEnterBarChartSeries
                        .RemoveAll(x => x.Name.Equals(item.Name, StringComparison.OrdinalIgnoreCase));
                    _dataEnterBarChartSeries.Add(new ChartSeries { Name = item.Name, Data = item.Data });
                }
            }
        }
    }
}