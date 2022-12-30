using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Features.Base.Commands;
using Coworkee.Domain.Entities.Localization;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Constants.Permission;
using MediatR;

namespace Coworkee.Application.Features.Translations.Commands.AddEdit
{
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
}