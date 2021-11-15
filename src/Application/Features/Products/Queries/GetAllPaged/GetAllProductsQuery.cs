using CleanArchitectureBase.Application.Specifications.Catalog;
using CleanArchitectureBase.Domain.Entities.Catalog;
using MediatR;
using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Application.Specifications.Base;

namespace CleanArchitectureBase.Application.Features.Products.Queries.GetAllPaged
{
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