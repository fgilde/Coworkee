using CleanArchitectureBase.Application.Features.Products.Queries.GetAllPaged;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.Products.Commands.AddEdit;
using CleanArchitectureBase.Shared.Wrapper;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Client.Pages.Catalog
{
    public partial class Products
    {
        
        [Parameter]
        public string Action { get; set; }

        [Parameter]
        public string Id { get; set; }

        
        private async Task<PaginatedResult<GetAllPagedProductsResponse>> Load(int pageNumber, int pageSize, string _searchString, string[] orderings)
        {
            return await _api.Products_GetAllAsync(pageNumber, pageSize, _searchString, orderings);
        }

        private async Task<GetAllPagedProductsResponse> FindById(int id, IEnumerable<GetAllPagedProductsResponse> loaded)
        {
            var res = loaded.FirstOrDefault(p => p.Id == id) ?? (await _api.Products_GetByIdAsync(id))?.Data;
            return res;
        }

        private int GetId(GetAllPagedProductsResponse product)
        {
            return product.Id;
        }

        private async Task<Result> DeleteProducts(int[] ids)
        {
            return await _api.Products_DeleteAsync(ids.ToList());
        }

        private string GetName(GetAllPagedProductsResponse arg)
        {
            return arg.Name;
        }

        private async Task<Result<string>> Export(string search)
        {
            return await _api.Products_ExportAsync(search);
        }

        private async Task<Result<string>> ExportSelected(int[] ids)
        {
            return await _api.Products_ExportByIdsAsync(ids.ToList());
        }

        private async Task<bool> CreateOrEditProduct(GetAllPagedProductsResponse productOrNull)
        {
            var parameters = new DialogParameters();
            if (productOrNull != null)
            {
                parameters.Add(nameof(AddEditProductModal.AddEditProductModel), productOrNull.MapTo<AddEditProductCommand>());
            }
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Medium, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<AddEditProductModal>(productOrNull == null ? _localizer["Create"] : _localizer["Edit"], parameters, options);
            var result = await dialog.Result;
            return !result.Cancelled;
        }
    }
}