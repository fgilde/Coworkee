using System;
using System.Threading;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Features.Base.Commands;
using Coworkee.Domain.Entities.Localization;
using Coworkee.Shared.Constants.Permission;
using MediatR;

namespace Coworkee.Application.Features.Translations.Commands.Delete
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