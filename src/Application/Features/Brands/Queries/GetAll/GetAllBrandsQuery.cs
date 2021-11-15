using CleanArchitectureBase.Domain.Entities.Catalog;
using LazyCache;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;

namespace CleanArchitectureBase.Application.Features.Brands.Queries.GetAll
{
    public class GetAllBrandsQuery : GetAllQueryBase<BrandDto>
    {}

    internal class GetAllBrandsQueryHandler : GetAllQueryHandlerBase<GetAllBrandsQuery, int, BrandDto, Brand>
    {
        public GetAllBrandsQueryHandler(IUnitOfWork<int> unitOfWork, IAppCache cache) : base(unitOfWork, cache)
        { }
    }
}