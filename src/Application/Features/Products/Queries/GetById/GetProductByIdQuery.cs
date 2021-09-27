using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Features.Products.Queries.GetAllPaged;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Domain.Entities.Catalog;
using CleanArchitectureBase.Shared.Wrapper;
using MediatR;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Products.Queries.GetById
{
    public class GetProductByIdQuery : IRequest<Result<GetAllPagedProductsResponse>>
    {
        public int Id { get; set; }

        public GetProductByIdQuery(int productId)
        {
            Id = productId;
        }
    }

    internal class GetProductQueryHandler : IRequestHandler<GetProductByIdQuery, Result<GetAllPagedProductsResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;

        public GetProductQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GetAllPagedProductsResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _unitOfWork.Repository<Product>().GetByIdAsync(request.Id);
            return await Result<GetAllPagedProductsResponse>.SuccessAsync(result?.MapTo<GetAllPagedProductsResponse>());

        }
    }
}