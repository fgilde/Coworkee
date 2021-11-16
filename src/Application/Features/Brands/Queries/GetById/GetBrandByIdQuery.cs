using System;
using CleanArchitectureBase.Domain.Entities.Catalog;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Shared.Constants.Permission;

namespace CleanArchitectureBase.Application.Features.Brands.Queries.GetById
{
    [CustomAuthorize(Policies = new[] { Permissions.Brands.View })]
    public class GetBrandByIdQuery : GetByIdQueryBase<int, BrandDto>
    {
        public GetBrandByIdQuery(int id) : base(id)
        { }
    }

    internal class GetBrandByIdQueryHandler : GetByIdQueryHandlerBase<GetBrandByIdQuery, int, BrandDto, Brand>
    {
        public GetBrandByIdQueryHandler(IUnitOfWork<int> unitOfWork, IServiceProvider provider) : base(unitOfWork, provider)
        { }
    }
}