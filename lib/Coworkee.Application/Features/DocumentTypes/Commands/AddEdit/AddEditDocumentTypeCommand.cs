using System;
using System.Linq;
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
using Microsoft.Extensions.Localization;

namespace lib.Coworkee.Application.Features.DocumentTypes.Commands.AddEdit;

[CustomAuthorize(Policies = new[] { CorePermissionProvider.Core.DocumentTypes.Create, CorePermissionProvider.Core.DocumentTypes.Edit }, PolicyMatch = PolicyMatch.Any)]
public class AddEditDocumentTypesCommand : AddEditCommandBase<DocumentTypeDto>
{
    public AddEditDocumentTypesCommand(params DocumentTypeDto[] items) : base(items)
    { }
}

internal class AddEditDocumentTypesCommandHandler : AddEditCommandHandlerBase<AddEditDocumentTypesCommand, int, DocumentTypeDto, DocumentType>
{
    private readonly IStringLocalizer<AddEditDocumentTypesCommandHandler> _localizer;
    protected override string EditPermission => CorePermissionProvider.Core.DocumentTypes.Edit;
    protected override string CreatePermission => CorePermissionProvider.Core.DocumentTypes.Create;

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