using System;
using System.Threading;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Features.Base.Commands;
using lib.Coworkee.Domain.Entities.Localization;
using lib.Coworkee.Shared.Constants.Permission;
using MediatR;

namespace Coworkee.Application.Features.Translations.Commands.Delete;

[CustomAuthorize(Policies = [Permissions.Translations.Delete])]
public class DeleteTranslationsCommand : DeleteCommandBase<int>
{}

internal class DeleteTranslationsCommandHandler : DeleteCommandHandlerBase<DeleteTranslationsCommand, int, TranslationDto, Translation>
{
    protected override string CacheKey => $"{base.CacheKey}-{Thread.CurrentThread.CurrentCulture.Name}";
    public DeleteTranslationsCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider) 
        : base(unitOfWork, mediator, permissionService, provider)
    {}
}