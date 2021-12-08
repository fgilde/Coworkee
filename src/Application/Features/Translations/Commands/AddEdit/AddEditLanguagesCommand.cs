using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Common.Security;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Domain.Entities.Localization;
using CleanArchitectureBase.Shared.Constants.Localization;
using CleanArchitectureBase.Shared.Constants.Permission;
using MediatR;

namespace CleanArchitectureBase.Application.Features.Translations.Commands.AddEdit
{
    [CustomAuthorize(Policies = new[] { Permissions.Translations.Create, Permissions.Translations.Edit }, PolicyMatch = PolicyMatch.Any)]
    public class AddEditLanguagesCommand : AddEditCommandBase<LanguageDto>
    {
        public bool AutoCreateTranslations { get; set; }
        public AddEditLanguagesCommand()
        { }

        public AddEditLanguagesCommand(params LanguageDto[] items) : base(items)
        { }
    }

    internal class AddEditLanguagesCommandHandler : AddEditCommandHandlerBase<AddEditLanguagesCommand, int, LanguageDto, Language>
    {
        public AddEditLanguagesCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider)
            : base(unitOfWork, mediator, permissionService, provider)
        { }

        public override async Task<AddUpdateResult<LanguageDto>> Handle(AddEditLanguagesCommand command, CancellationToken cancellationToken)
        {
            var res = await base.Handle(command, cancellationToken);
            if (command.AutoCreateTranslations)
                await Translate(command, cancellationToken);
            return res;
        }

        private async Task Translate(AddEditLanguagesCommand command, CancellationToken cancellationToken = default)
        {
            var toCreate = new List<TranslationDto>();
            var inputDictionary = LocalizationConstants.GetDefaultLanguageResources();
            var targets = command.Items.Where(d => d.IsActive).Select(dto => dto.CultureCode).ToList();
            if (targets.Any())
            {
                var res = await Get<ITranslationService>().TranslateResourceDictionaryAsync(inputDictionary, targets.ToArray()).ConfigureAwait(false);
                foreach (var result in res)
                {
                    var existing = UnitOfWork.Repository<Translation>().Entities.Where(t => t.CultureCode == result.Key);
                    var missing = result.Value.Select(p => new TranslationDto { Key = p.Key, Value = p.Value, CultureCode = result.Key })
                        .Where(tr => existing.All(t => t.CultureCode != tr.CultureCode && t.Key != tr.Key));
                    toCreate.AddRange(missing);
                }
                if(toCreate.Any())
                    await Mediator.Send(new AddEditTranslationsCommand(toCreate.ToArray()), cancellationToken);
            }
        }
    }
}