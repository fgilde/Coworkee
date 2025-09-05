using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Contracts.Enums;
using lib.Coworkee.Application.Features.Brands.Commands.AddEdit;
using lib.Coworkee.Application.Hubs;
using Coworkee.Client.Extensions;
using lib.Coworkee.Shared.Wrapper;
using Microsoft.AspNetCore.SignalR.Client;

namespace Coworkee.Client.Pages.Catalog
{
    public partial class Brands
    {
        [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Parameter] public string Action { get; set; }

        [Parameter] public string Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            HubConnection = await HubConnection.EnsureStartedAsync(_config.BackendOrigin);
            await base.OnInitializedAsync();
        }
        
        private async Task<Result<List<BrandDto>>> Load(CancellationToken cancellationToken)
        {            
            return new Result<List<BrandDto>>()
            {
                Succeeded = true,
                Data = (await _api.Brands_GetAllAsync(cancellationToken: cancellationToken)).ToList()
            };
        }

        private Task<BrandDto> FindById(string id, IEnumerable<BrandDto> loaded)
        {
            return Task.FromResult(loaded.FirstOrDefault(p => p.Id == id));
        }

        private string GetId(BrandDto brand) 
            => brand.Id;

        private async Task<Result> DeleteBrands(string[] ids)
        {
            await _api.Brands_DeleteAsync(ids.ToList());
            return new Result {Succeeded = true};
        }

        private string GetName(BrandDto arg) 
            => arg.Name;

        private async Task Export(ExportServiceType serviceType, string search)
        {
            await (await _api.Brands_ExportAsync(exportServiceType: serviceType, searchString: search)).ForceDownloadAsync(_jsRuntime);
        }

        private async Task ExportSelected(ExportServiceType serviceType, string[] ids)
        {
            await(await _api.Brands_ExportAsync(exportServiceType: serviceType, ids: ids)).ForceDownloadAsync(_jsRuntime);
        }

        private async Task<bool> CreateOrEditBrand(BrandDto brandOrNull)
        {
            return !(await _dialogService.EditOrCreate(brandOrNull, async (dto, _) =>
            {
                await _api.Brands_PostAsync(new AddEditBrandsCommand(dto));
                await HubConnection.TrySendAsync(_config.BackendOrigin, nameof(ClientEventHub.UpdateDashboardAsync));
                return null;
            })).Cancelled;
        }
    }
}