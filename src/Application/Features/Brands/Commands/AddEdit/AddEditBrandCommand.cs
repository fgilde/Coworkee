using System;
using Coworkee.Domain.Entities.Catalog;
using MediatR;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Features.Base.Commands;
using lib.Coworkee.Shared.Constants.Permission;

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