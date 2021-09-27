using System.Linq;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Domain.Entities.Catalog;
using CleanArchitectureBase.Shared.Wrapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Products.Commands.Delete
{
    public class DeleteProductCommand : IRequest<IResult>
    {
        public int[] Ids { get; set; }
    }

    internal class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, IResult>
    {
        private readonly IUnitOfWork<int> _unitOfWork;
        private readonly IStringLocalizer<DeleteProductCommandHandler> _localizer;

        public DeleteProductCommandHandler(IUnitOfWork<int> unitOfWork, IStringLocalizer<DeleteProductCommandHandler> localizer)
        {
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<IResult> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            var products = await Task.WhenAll(command.Ids.Select(id => _unitOfWork.Repository<Product>().GetByIdAsync(id)));
            if (products.Any())
            {
                await _unitOfWork.Repository<Product>().DeleteManyAsync(products);
                await _unitOfWork.Commit(cancellationToken);
                return await Result.SuccessAsync(_localizer["Products Deleted"]);
            }

            return await Result.FailAsync(_localizer["Products Not Found!"]);
        }
    }
}