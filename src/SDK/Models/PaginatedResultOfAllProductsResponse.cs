using System.Collections.Generic;
using CleanArchitectureBase.Application.Features.Products.Queries.GetAllPaged;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.SDK.Models
{
    public class PaginatedResultOfGetAllPagedProductsResponse: PaginatedResult<GetAllPagedProductsResponse>
    {
        public PaginatedResultOfGetAllPagedProductsResponse(List<GetAllPagedProductsResponse> data) : base(data)
        { }
    }
}