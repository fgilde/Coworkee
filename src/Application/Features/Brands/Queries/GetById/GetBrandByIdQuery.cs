using System;
using Coworkee.Domain.Entities.Catalog;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Shared.Constants.Permission;

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