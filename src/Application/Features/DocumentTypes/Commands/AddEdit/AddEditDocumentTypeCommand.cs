using System;
using System.Linq;
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
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.DocumentTypes.Commands.AddEdit
{

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

        public override async Task<Unit> Handle(AddEditDocumentTypesCommand command, CancellationToken cancellationToken)
        {
            if (command.Items.Any(item => UnitOfWork.Repository<DocumentType>().Entities.Any(p => p.Id != item.Id && p.Name == item.Name)))
                throw Errors.Create(_localizer["Document type with this name already exists."], HttpStatusCode.Conflict);

            await base.Handle(command, cancellationToken);
            return Unit.Value;
        }
    }
}