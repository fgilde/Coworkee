using System;
using System.Net;
using Coworkee.Domain.Entities.Catalog;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Features.Base.Commands;
using Coworkee.Shared.Constants.Permission;

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