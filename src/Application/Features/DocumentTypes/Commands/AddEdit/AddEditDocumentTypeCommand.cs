using System;
using System.Linq;
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
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Features.DocumentTypes.Commands.AddEdit;

[CustomAuthorize(Policies = new[] { Permissions.DocumentTypes.Create, Permissions.DocumentTypes.Edit }, PolicyMatch = PolicyMatch.Any)]
public class AddEditDocumentTypesCommand : AddEditCommandBase<DocumentTypeDto>
{
    public AddEditDocumentTypesCommand(params DocumentTypeDto[] items) : base(items)
    { }
}

internal class AddEditDocumentTypesCommandHandler : AddEditCommandHandlerBase<AddEditDocumentTypesCommand, int, DocumentTypeDto, DocumentType>
{
    private readonly IStringLocalizer<AddEditDocumentTypesCommandHandler> _localizer;
    protected override string EditPermission => Permissions.DocumentTypes.Edit;
    protected override string CreatePermission => Permissions.DocumentTypes.Create;

    public AddEditDocumentTypesCommandHandler(
        IUnitOfWork<int> unitOfWork,
        IMediator mediator,
        IPermissionService permissionService,
        IServiceProvider provider,
        IStringLocalizer<AddEditDocumentTypesCommandHandler> localizer)
        : base(unitOfWork, mediator, permissionService, provider)
    {
        _localizer = localizer;
    }

    public override async Task<AddUpdateResult<DocumentTypeDto>> Handle(AddEditDocumentTypesCommand command, CancellationToken cancellationToken)
    {
        if (command.Items.Any(item => UnitOfWork.Repository<DocumentType>().Entities.Any(p => p.Id != item.Id && p.Name == item.Name)))
            throw Errors.Create(_localizer["Document type with this name already exists."], HttpStatusCode.Conflict);

        return await base.Handle(command, cancellationToken);
    }
}