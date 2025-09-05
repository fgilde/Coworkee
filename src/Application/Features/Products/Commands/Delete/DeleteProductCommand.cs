using System;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services;
using Coworkee.Domain.Entities.Catalog;
using MediatR;
using lib.Coworkee.Application.Features.Base.Commands;
using lib.Coworkee.Shared.Constants.Permission;

namespace Coworkee.Application.Features.Products.Commands.Delete
{
    [CustomAuthorize(Policies = new[] { Permissions.Products.Delete })]
    public class DeleteProductCommand : DeleteCommandBase<int>
    { }

    internal class DeleteProductCommandHandler : DeleteCommandHandlerBase<DeleteProductCommand, int, ProductDto, Product>
    {
        public DeleteProductCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider)
            : base(unitOfWork, mediator, permissionService, provider)
        { }
    }
}