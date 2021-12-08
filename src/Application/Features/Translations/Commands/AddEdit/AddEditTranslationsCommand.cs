using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Domain.Entities.Localization;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Constants.Permission;
using MediatR;

namespace CleanArchitectureBase.Application.Features.Translations.Commands.AddEdit
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