using Coworkee.Domain.Entities.Catalog;
using LazyCache;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Shared.Constants.Permission;

namespace Coworkee.Application.Features.Brands.Queries.GetAll
{
    [CustomAuthorize(Policies = new[] { Permissions.Brands.View })]
    public class GetAllBrandsQuery : GetAllQueryBase<BrandDto>
    {}

    internal class GetAllBrandsQueryHandler : GetAllQueryHandlerBase<GetAllBrandsQuery, int, BrandDto, Brand>
    {
        public GetAllBrandsQueryHandler(IUnitOfWork<int> unitOfWork, IAppCache cache) : base(unitOfWork, cache)
        { }
    }
}