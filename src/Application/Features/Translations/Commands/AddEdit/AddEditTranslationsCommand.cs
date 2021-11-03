using System;
using System.Threading;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Domain.Entities.Localization;
using MediatR;

namespace CleanArchitectureBase.Application.Features.Translations.Commands.AddEdit
{
    //[CustomAuthorize(Policies = new[] { Permissions.Products.Create, Permissions.Products.Edit }, PolicyMatch = PolicyMatch.Any)]
    public class AddEditTranslationsCommand : AddEditCommandBase<TranslationDto>
    {}

    internal class AddEditTranslationsCommandCommandHandler : AddEditCommandHandlerBase<AddEditTranslationsCommand, int, TranslationDto, Translation>
    {
        protected override string CacheKey => $"{base.CacheKey}-{Thread.CurrentThread.CurrentCulture.Name}";
        public AddEditTranslationsCommandCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider) 
            : base(unitOfWork, mediator, permissionService, provider)
        { }
    }
}