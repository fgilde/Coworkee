using System;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Domain.Entities.Catalog;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Application.Security;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Products.Commands.AddEdit
{
    [CustomAuthorize(Policies = new[] { Permissions.Products.Create, Permissions.Products.Edit }, PolicyMatch = PolicyMatch.Any)]
    public class AddEditProductsCommand : AddEditCommandBase<UpdateProductDto>
    {
        public AddEditProductsCommand(params UpdateProductDto[] items) : base(items)
        {}
    }

    internal class AddEditProductsCommandHandler : AddEditCommandHandlerBase<AddEditProductsCommand, int, UpdateProductDto, Product>
    {
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditProductsCommandHandler> _localizer;
        protected override string EditPermission => Permissions.Products.Edit;
        protected override string CreatePermission => Permissions.Products.Create;

        public AddEditProductsCommandHandler(
            IUnitOfWork<int> unitOfWork, 
            IMediator mediator, 
            IPermissionService permissionService, 
            IServiceProvider provider, 
            IUploadService uploadService, IStringLocalizer<AddEditProductsCommandHandler> localizer)
            : base(unitOfWork, mediator, permissionService, provider)
        {
            _uploadService = uploadService;
            _localizer = localizer;
        }

        public override async Task<Unit> Handle(AddEditProductsCommand command, CancellationToken cancellationToken)
        {
            if (command.Items.Any(item => UnitOfWork.Repository<Product>().Entities.Any(p => p.Id != item.Id && p.Barcode == item.Barcode)))
                throw Errors.Create(_localizer["Barcode already exists."], HttpStatusCode.Conflict);
            
            var uploadTasks = command.Items.Where(dto => dto.UploadRequest != null).Select(dto =>
                Task.Run(() => _uploadService.UploadAsync(dto.UploadRequest), cancellationToken)
                    .ContinueWith(task => dto.ImageDataURL = task.Result, cancellationToken));
            await Task.WhenAll(uploadTasks);
            await base.Handle(command, cancellationToken);
            return Unit.Value;
        }
    }
}
