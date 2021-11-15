using System;
using CleanArchitectureBase.Domain.Entities.Catalog;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;

namespace CleanArchitectureBase.Application.Features.Brands.Commands.Delete
{
    public class DeleteBrandCommand : DeleteCommandBase<int>
    { }

    internal class DeleteBrandCommandHandler : DeleteCommandHandlerBase<DeleteBrandCommand, int, BrandDto, Brand>
    {
        public DeleteBrandCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider)
            : base(unitOfWork, mediator, permissionService, provider)
        { }

        public override async Task<Unit> Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
        {
            var productRepository = Get<IProductRepository>();
            foreach (var id in command.Ids)
            {
                if (await productRepository.IsBrandUsed(id))
                    throw new Exception("Deletion Not Allowed");
            }
            return await base.Handle(command, cancellationToken);
        }
    }
}