using System;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Domain.Entities.Catalog;
using MediatR;
using Coworkee.Application.Features.Base.Commands;
using Coworkee.Shared.Constants.Permission;

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