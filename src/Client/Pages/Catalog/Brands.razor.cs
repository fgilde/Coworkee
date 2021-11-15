using CleanArchitectureBase.Application.Features.Brands.Queries.GetAll;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
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


        private async Task<Result<List<BrandDto>>> Load()
        {
            return new Result<List<BrandDto>>()
            {
                Succeeded = true,
                Data = (await _api.Brands_GetAllAsync()).ToList()
            };
        }

        private Task<BrandDto> FindById(int id, IEnumerable<BrandDto> loaded)
        {
            return Task.FromResult(loaded.FirstOrDefault(p => p.Id == id));
        }

        private int GetId(BrandDto brand)
        {
            return brand.Id;
        }

        private async Task<Result> DeleteBrands(int[] ids)
        {
            await _api.Brands_DeleteAsync(ids.ToList());
            return new Result {Succeeded = true};
        }

        private string GetName(BrandDto arg)
        {
            return arg.Name;
        }

        private async Task<Result<string>> Export(string search)
        {
            return await _api.Brands_ExportAsync(search);
        }

        private Task<Result<string>> ExportSelected(int[] ids)
        {
            throw new NotImplementedException("Not implemented");
            //return await _api.Products_ExportByIdsAsync(ids.ToList());
        }

        private async Task<bool> CreateOrEditBrand(BrandDto brandOrNull)
        {
            var parameters = new DialogParameters();
            if (brandOrNull != null)
            {
                parameters.Add(nameof(AddEditBrandModal.AddEditBrandModel), brandOrNull.MapTo<BrandDto>());
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditBrandModal>(brandOrNull == null ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;

            return !result.Cancelled;
        }
    }
}