using System.Collections.Generic;
using CleanArchitectureBase.Application.Features.Brands.Queries.GetAll;
using CleanArchitectureBase.Application.Features.Dashboards.Queries.GetData;
using CleanArchitectureBase.Application.Features.Products.Queries.GetAllPaged;
using CleanArchitectureBase.Application.Responses.Audit;
using CleanArchitectureBase.Application.Responses.Identity;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.SDK.Models
{
    public class ResultOfTokenResponse: Result<TokenResponse> {}
    public class ResultOfInteger : Result<int> { }
    public class ResultOfString : Result<string> { }
    public class ResultOfListOfGetAllBrandsResponse : Result<List<GetAllBrandsResponse>> { }
    public class ResultOfGetBrandByIdResponse : Result<ResultOfGetBrandByIdResponse> { }
    public class ResultOfIEnumerableOfAuditResponse: Result<IEnumerable<AuditResponse>> {}
    public class ResultOfIEnumerableOfChatUserResponse : Result<IEnumerable<ChatUserResponse>> {}
    public class ResultOfIEnumerableOfChatHistoryResponse : Result<IEnumerable<ChatHistoryResponse>> {}
    public class ResultOfDashboardDataResponse: Result<DashboardDataResponse> {}

    public class PaginatedResultOfGetAllPagedProductsResponse : PaginatedResult<GetAllPagedProductsResponse>
    {
        public PaginatedResultOfGetAllPagedProductsResponse(List<GetAllPagedProductsResponse> data) : base(data)
        { }
    }

}