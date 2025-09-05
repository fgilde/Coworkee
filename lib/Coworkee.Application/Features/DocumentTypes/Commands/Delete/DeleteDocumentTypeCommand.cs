using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Features.Base.Commands;
using Coworkee.Domain.Entities.Misc;
using Coworkee.Shared.Constants.Permission;
using MediatR;

namespace Coworkee.Application.Features.DocumentTypes.Commands.Delete;

[CustomAuthorize(Policies = new[] { Permissions.DocumentTypes.Delete })]
public class DeleteDocumentTypesCommand : DeleteCommandBase<int>
{ }

internal class DeleteDocumentTypesCommandHandler : DeleteCommandHandlerBase<DeleteDocumentTypesCommand, int, DocumentTypeDto, DocumentType>
{
    public DeleteDocumentTypesCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider)
        : base(unitOfWork, mediator, permissionService, provider)
    { }

    public override async Task Handle(DeleteDocumentTypesCommand command, CancellationToken cancellationToken)
    {
        var productRepository = Get<IDocumentRepository>();
        foreach (var id in command.Ids)
        {
            if (await productRepository.IsDocumentTypeUsed(id))
                throw Errors.Create("Deletion Not Allowed", HttpStatusCode.Conflict);
        }
        await base.Handle(command, cancellationToken);
    }
}