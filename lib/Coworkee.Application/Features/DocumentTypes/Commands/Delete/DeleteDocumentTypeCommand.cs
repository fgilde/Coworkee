using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Features.Base.Commands;
using lib.Coworkee.Domain.Entities.Misc;
using lib.Coworkee.Shared.Constants.Permission;
using MediatR;

namespace lib.Coworkee.Application.Features.DocumentTypes.Commands.Delete;

[CustomAuthorize(Policies = new[] { CorePermissionProvider.Core.DocumentTypes.Delete })]
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