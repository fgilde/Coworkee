using System;
using System.Threading;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Domain.Entities.Localization;
using CleanArchitectureBase.Shared.Constants.Permission;
using MediatR;

namespace CleanArchitectureBase.Application.Features.Translations.Commands.Delete
{
    [CustomAuthorize(Policies = new[] { Permissions.Translations.Delete })]
    public class DeleteTranslationsCommand : DeleteCommandBase<int>
    {}

    internal class DeleteTranslationsCommandHandler : DeleteCommandHandlerBase<DeleteTranslationsCommand, int, TranslationDto, Translation>
    {
        protected override string CacheKey => $"{base.CacheKey}-{Thread.CurrentThread.CurrentCulture.Name}";
        public DeleteTranslationsCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider) 
            : base(unitOfWork, mediator, permissionService, provider)
        {}
    }
}