using CleanArchitectureBase.Domain.Entities.Catalog;
using CleanArchitectureBase.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Repositories;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Brands.Queries.GetById
{
    public class GetBrandByIdQuery : IRequest<Result<GetBrandByIdResponse>>
    {
        public int Id { get; set; }
    }

    internal class GetProductByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<GetBrandByIdResponse>>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        
        public GetProductByIdQueryHandler(IUnitOfWork<int> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GetBrandByIdResponse>> Handle(GetBrandByIdQuery query, CancellationToken cancellationToken)
        {
            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(query.Id);
            var mappedBrand = brand.MapTo<GetBrandByIdResponse>();
            return await Result<GetBrandByIdResponse>.SuccessAsync(mappedBrand);
        }
    }
}