using Coworkee.Application.Specifications.Catalog;
using Coworkee.Domain.Entities.Catalog;
using MediatR;
using System;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Application.Specifications.Base;
using lib.Coworkee.Shared.Constants.Permission;

namespace Coworkee.Application.Features.Products.Queries.GetAllPaged
{
    [CustomAuthorize(Policies = new[] { Permissions.Products.View })]
    public class GetAllProductsQuery : GetAllPagedQueryBase<ProductDto>
    {}

    internal class GetAllProductsQueryHandler : GetAllPagedQueryHandlerBase<GetAllProductsQuery, int, ProductDto, Product>
    {
        protected override ISpecification<Product> GetFilterSpecification(GetAllProductsQuery query)
        {
            return new ProductFilterSpecification(query.SearchString);
        }

        public GetAllProductsQueryHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IServiceProvider provider)
            : base(unitOfWork, mediator, provider)
        { }
    }
}