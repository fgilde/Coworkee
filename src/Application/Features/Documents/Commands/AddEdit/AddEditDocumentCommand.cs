using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Features.Base.Commands;
using Coworkee.Domain.Entities.Misc;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace Coworkee.Application.Features.Documents.Commands.AddEdit
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
            var documentsWithoutType = command.Items.Where(dto => dto.DocumentTypeId == default).ToList();
            var documentTypeForUnassigned = documentsWithoutType.Any() ? await GetForUnassigned(cancellationToken) : null;
            foreach (var documentDto in documentsWithoutType)
                documentDto.DocumentTypeId = documentTypeForUnassigned?.Id ?? default;

            return await base.Handle(command, cancellationToken);
        }

        private async Task<DocumentType> GetForUnassigned(CancellationToken cancellationToken)
        {
            DocumentType documentType = UnitOfWork.Repository<DocumentType>().Entities.FirstOrDefault(type => type.Name == ApplicationConstants.DefaultDocumentTypeName);
            if (documentType == null)
            {
                documentType = await UnitOfWork.Repository<DocumentType>().AddAsync(new DocumentType
                {
                    Description = _localizer["All Documents without Type are stored here"],
                    Name = ApplicationConstants.DefaultDocumentTypeName
                }, cancellationToken);
                await UnitOfWork.Commit(cancellationToken);
            }

            return documentType;
        }

    }
}