using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Catalog;
using CleanArchitectureBase.Shared.Constants.Permission;

namespace CleanArchitectureBase.Application.Features.Products.Queries.GetById
{
    [CustomAuthorize(Policies = new[] { Permissions.Products.View })]
    public class GetProductByIdQuery : GetByIdQueryBase<int, ProductDto>
    {
        public GetProductByIdQuery(int id) : base(id)
        { }
    }

    internal class GetProductByIdQueryHandler : GetByIdQueryHandlerBase<GetProductByIdQuery, int, ProductDto, Product>
    {
        public GetProductByIdQueryHandler(IUnitOfWork<int> unitOfWork, IServiceProvider provider) : base(unitOfWork, provider)
        { }
    }
}