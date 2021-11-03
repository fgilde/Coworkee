using System;
using System.Threading;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Application.Interfaces.Services;
using CleanArchitectureBase.Domain.Entities.Localization;
using MediatR;

namespace CleanArchitectureBase.Application.Features.Translations.Commands.Delete
{
    public class DeleteTranslationsCommand : DeleteCommandBase<int>
    {}

    internal class DeleteTranslationsCommandHandler : DeleteCommandHandlerBase<DeleteTranslationsCommand, int, Translation>
    {
        protected override string CacheKey => $"{base.CacheKey}-{Thread.CurrentThread.CurrentCulture.Name}";
        public DeleteTranslationsCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider) 
            : base(unitOfWork, mediator, permissionService, provider)
        {}
    }
}