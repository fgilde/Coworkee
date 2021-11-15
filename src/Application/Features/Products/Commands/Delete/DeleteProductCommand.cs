using System;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Domain.Entities.Catalog;
using MediatR;
using CleanArchitectureBase.Application.Features.Base.Commands;

namespace CleanArchitectureBase.Application.Features.Products.Commands.Delete
{
    public class DeleteProductCommand : DeleteCommandBase<int>
    { }

    internal class DeleteProductCommandHandler : DeleteCommandHandlerBase<DeleteProductCommand, int, ProductDto, Product>
    {
        public DeleteProductCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider)
            : base(unitOfWork, mediator, permissionService, provider)
        { }
    }
}