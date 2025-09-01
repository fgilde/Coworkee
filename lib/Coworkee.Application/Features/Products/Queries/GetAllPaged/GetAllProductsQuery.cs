using Coworkee.Application.Specifications.Catalog;
using Coworkee.Domain.Entities.Catalog;
using MediatR;
using System;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Application.Specifications.Base;
using Coworkee.Shared.Constants.Permission;

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