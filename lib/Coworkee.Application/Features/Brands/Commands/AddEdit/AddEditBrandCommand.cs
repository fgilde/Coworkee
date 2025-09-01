using System;
using Coworkee.Domain.Entities.Catalog;
using MediatR;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Features.Base.Commands;
using Coworkee.Shared.Constants.Permission;

namespace Coworkee.Application.Features.Brands.Commands.AddEdit
{
    [CustomAuthorize(Policies = new[] { Permissions.Brands.Create, Permissions.Brands.Edit }, PolicyMatch = PolicyMatch.Any)]
    public class AddEditBrandsCommand : AddEditCommandBase<BrandDto>
    {
        public AddEditBrandsCommand(params BrandDto[] items) : base(items)
        { }
    }

    internal class AddEditBrandCommandHandler : AddEditCommandHandlerBase<AddEditBrandsCommand, int, BrandDto, Brand>
    {
        protected override string EditPermission => Permissions.Brands.Edit;
        protected override string CreatePermission => Permissions.Brands.Create;
        public AddEditBrandCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider)
            : base(unitOfWork, mediator, permissionService, provider)
        { }
    }
}