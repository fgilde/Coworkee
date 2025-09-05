using System;
using System.Net;
using Coworkee.Domain.Entities.Catalog;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Features.Base.Commands;
using lib.Coworkee.Shared.Constants.Permission;

namespace Coworkee.Application.Features.Brands.Commands.Delete
{
    [CustomAuthorize(Policies = new[] { Permissions.Brands.Delete })]
    public class DeleteBrandCommand : DeleteCommandBase<int>
    { }

    internal class DeleteBrandCommandHandler : DeleteCommandHandlerBase<DeleteBrandCommand, int, BrandDto, Brand>
    {
        public DeleteBrandCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider)
            : base(unitOfWork, mediator, permissionService, provider)
        { }

        public override async Task Handle(DeleteBrandCommand command, CancellationToken cancellationToken)
        {
            var productRepository = Get<IProductRepository>();
            foreach (var id in command.Ids)
            {
                if (await productRepository.IsBrandUsed(id))
                    throw Errors.Create("Deletion Not Allowed", HttpStatusCode.Conflict);
            }
            await base.Handle(command, cancellationToken);
        }
    }
}