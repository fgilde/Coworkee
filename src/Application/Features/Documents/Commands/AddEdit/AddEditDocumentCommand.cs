using CleanArchitectureBase.Domain.Entities.Misc;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Documents.Commands.AddEdit
{
    [CustomAuthorize(Policies = new[] { Permissions.Documents.Create, Permissions.Documents.Edit }, PolicyMatch = PolicyMatch.Any)]
    public class AddEditDocumentsCommand : AddEditCommandBase<DocumentDto>
    {
        public AddEditDocumentsCommand(params DocumentDto[] items) : base(items)
        { }
    }

    internal class AddEditDocumentsCommandHandler : AddEditCommandHandlerBase<AddEditDocumentsCommand, int, DocumentDto, Document>
    {
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditDocumentsCommandHandler> _localizer;
        protected override string EditPermission => Permissions.Documents.Edit;
        protected override string CreatePermission => Permissions.Documents.Create;

        public AddEditDocumentsCommandHandler(
            IUnitOfWork<int> unitOfWork,
            IMediator mediator,
            IPermissionService permissionService,
            IServiceProvider provider,
            IUploadService uploadService, IStringLocalizer<AddEditDocumentsCommandHandler> localizer)
            : base(unitOfWork, mediator, permissionService, provider)
        {
            _uploadService = uploadService;
            _localizer = localizer;
        }

        public override async Task<AddUpdateResult<DocumentDto>> Handle(AddEditDocumentsCommand command, CancellationToken cancellationToken)
        {
            var uploadTasks = command.Items.Where(dto => dto.UploadRequest != null).Select(dto =>
                Task.Run(() => _uploadService.UploadAsync(dto.UploadRequest), cancellationToken)
                    .ContinueWith(task => dto.URL = task.Result, cancellationToken));
            await Task.WhenAll(uploadTasks);
            return await base.Handle(command, cancellationToken);
        }
    }
}