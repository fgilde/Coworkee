using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Application.Requests;
using CleanArchitectureBase.Domain.Entities.Catalog;
using CleanArchitectureBase.Shared.Constants.Permission;
using Nextended.Core;

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

        public override async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await base.Handle(request, cancellationToken);
            // TODO: Change ImageUrl to Document Entity
            if (!string.IsNullOrWhiteSpace(result.ImageDataURL))
                result.UploadRequest = await UploadRequest.FromUrlAsync(result.ImageDataURL, cancellationToken);
            
            return result;
        }
    }
}