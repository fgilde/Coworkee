using System;
using Coworkee.Domain.Entities.Catalog;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Shared.Constants.Permission;

namespace Coworkee.Application.Features.Brands.Queries.GetById
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