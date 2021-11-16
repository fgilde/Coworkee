using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Domain.Entities.Misc;
using CleanArchitectureBase.Shared.Constants.Permission;
using MediatR;

namespace CleanArchitectureBase.Application.Features.DocumentTypes.Commands.Delete
{
    [CustomAuthorize(Policies = new[] { Permissions.DocumentTypes.Delete })]
    public class DeleteDocumentTypesCommand : DeleteCommandBase<int>
    { }

    internal class DeleteDocumentTypesCommandHandler : DeleteCommandHandlerBase<DeleteDocumentTypesCommand, int, DocumentTypeDto, DocumentType>
    {
        public DeleteDocumentTypesCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider)
            : base(unitOfWork, mediator, permissionService, provider)
        { }

        public override async Task<Unit> Handle(DeleteDocumentTypesCommand command, CancellationToken cancellationToken)
        {
            var productRepository = Get<IDocumentRepository>();
            foreach (var id in command.Ids)
            {
                if (await productRepository.IsDocumentTypeUsed(id))
                    throw Errors.Create("Deletion Not Allowed", HttpStatusCode.Conflict);
            }
            return await base.Handle(command, cancellationToken);
        }
    }
}