using System;
using CleanArchitectureBase.Domain.Entities.Catalog;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Shared.Constants.Permission;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Products.Commands.AddEdit
{
    [CustomAuthorize(Policies = new[] { Permissions.Products.Create, Permissions.Products.Edit }, PolicyMatch = PolicyMatch.Any)]
    public class AddEditProductsCommand : AddEditCommandBase<ProductDto>
    {
        public AddEditProductsCommand(params ProductDto[] items) : base(items)
        {}
    }

    internal class AddEditProductsCommandHandler : AddEditCommandHandlerBase<AddEditProductsCommand, int, ProductDto, Product>
    {
        private readonly IUploadService _uploadService;
        private readonly IStringLocalizer<AddEditProductsCommandHandler> _localizer;
        protected override string EditPermission => Permissions.Products.Edit;
        protected override string CreatePermission => Permissions.Products.Create;
        //protected override Expression<Func<Product, object>>[] Includes => new Expression<Func<Product, object>>[] { c => c.Barcode };

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

        protected override Product ToEntity(ProductDto dto)
        {
            var res = base.ToEntity(dto);
            res.Brand = UnitOfWork.Repository<Brand>().GetById(dto.Brand.GetId());
            res.BrandId = res.Brand.Id;
            return res;
        }

        public override async Task<AddUpdateResult<ProductDto>> Handle(AddEditProductsCommand command, CancellationToken cancellationToken)
        {
            if (command.Items.Any(item => UnitOfWork.Repository<Product>().Entities.Any(p => p.Id != item.GetId() && p.Barcode == item.Barcode)))
                throw Errors.Create(_localizer["Barcode already exists."], HttpStatusCode.Conflict);

            var uploadTasks = command.Items.Where(dto => dto.UploadRequest != null).Select(dto =>
                    _uploadService.UploadAsync(dto.UploadRequest, cancellationToken)
                    .ContinueWith(task => dto.ImageDataURL = task.Result, cancellationToken));
            await Task.WhenAll(uploadTasks);
            return await base.Handle(command, cancellationToken);
        }
    }
}
