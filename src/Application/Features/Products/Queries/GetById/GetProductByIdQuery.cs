using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Application.Requests;
using Coworkee.Domain.Entities.Catalog;
using lib.Coworkee.Shared.Constants.Permission;
using Nextended.Core;

namespace Coworkee.Application.Features.Products.Queries.GetById
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