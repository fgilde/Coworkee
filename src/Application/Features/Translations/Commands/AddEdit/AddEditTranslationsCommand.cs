using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Common.Security;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services;
using lib.Coworkee.Application.Features.Base.Commands;
using lib.Coworkee.Domain.Entities.Localization;
using lib.Coworkee.Shared.Constants.Application;
using lib.Coworkee.Shared.Constants.Permission;
using MediatR;

namespace Coworkee.Application.Features.Translations.Commands.AddEdit;

[CustomAuthorize(Policies = new[] { Permissions.Translations.Create, Permissions.Translations.Edit }, PolicyMatch = PolicyMatch.Any)]
public class AddEditTranslationsCommand : AddEditCommandBase<TranslationDto>
{
    public AddEditTranslationsCommand()
    { }

    public AddEditTranslationsCommand(params TranslationDto[] items) : base(items)
    { }
}

internal class AddEditTranslationsCommandHandler : AddEditCommandHandlerBase<AddEditTranslationsCommand, int, TranslationDto, Translation>
{
    protected override IEnumerable<string> CacheKeys(AddEditTranslationsCommand command)
    {
        return command.Items.Select(d => d.CultureCode).Concat(new[] {Thread.CurrentThread.CurrentCulture.Name}).Distinct()
            .Select(s => ApplicationConstants.Cache.CacheKeyFor(typeof(Translation), s));
    }

    public AddEditTranslationsCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider) 
        : base(unitOfWork, mediator, permissionService, provider)
    { }
}