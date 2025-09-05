using Coworkee.Domain.Entities.Catalog;
using LazyCache;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Shared.Constants.Permission;

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