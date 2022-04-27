using CleanArchitectureBase.Application.Features.Brands.Queries.GetAll;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Client.Extensions;
using CleanArchitectureBase.Shared;
using CleanArchitectureBase.Shared.Wrapper;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Pages.Catalog
{
    public partial class Brands
    {

        [Parameter]
        public string Action { get; set; }

        [Parameter]
        public string Id { get; set; }


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
        {
            return brand.Id;
        }

        private async Task<Result> DeleteBrands(string[] ids)
        {
            await _api.Brands_DeleteAsync(ids.ToList());
            return new Result {Succeeded = true};
        }

        private string GetName(BrandDto arg)
        {
            return arg.Name;
        }

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
            var parameters = new DialogParameters();
            if (brandOrNull != null)
            {
                parameters.Add(nameof(AddEditBrandModal.AddEditBrandModel), brandOrNull.MapTo<BrandDto>());
            }
            
            var dialog = await _dialogService.ShowWithDefaultOptionsAsync<AddEditBrandModal>(brandOrNull == null ? _localizer["Create"] : _localizer["Edit"], parameters);
            var result = await dialog.Result;

            return !result.Cancelled;
        }
    }
}