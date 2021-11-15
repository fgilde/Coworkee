using System;
using CleanArchitectureBase.Domain.Entities.Catalog;
using MediatR;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Shared.Constants.Permission;

namespace CleanArchitectureBase.Application.Features.Brands.Commands.AddEdit
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