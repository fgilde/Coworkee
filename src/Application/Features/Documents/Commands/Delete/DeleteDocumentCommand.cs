using System;
using System.Linq;
using CleanArchitectureBase.Domain.Entities.Misc;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services;
using CleanArchitectureBase.Application.Features.Base.Commands;
using CleanArchitectureBase.Shared.Constants.Application;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureBase.Application.Features.Documents.Commands.Delete
{

    public class DeleteDocumentsCommand : DeleteCommandBase<int>
    { }

    internal class DeleteDocumentsCommandHandler : DeleteCommandHandlerBase<DeleteDocumentsCommand, int, DocumentDto, Document>
    {
        private readonly IAppCache _cache;

        public DeleteDocumentsCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider, IAppCache cache)
            : base(unitOfWork, mediator, permissionService, provider)
        {
            _cache = cache;
        }

        public override async Task<Unit> Handle(DeleteDocumentsCommand command, CancellationToken cancellationToken)
        {
            var documentsWithExtendedAttributes = UnitOfWork.Repository<Document>().Entities.Include(x => x.ExtendedAttributes);
            
            var cacheKeys = await documentsWithExtendedAttributes.SelectMany(x => x.ExtendedAttributes).Where(x => command.Ids.Contains(x.EntityId))
                .Distinct().Select(x => ApplicationConstants.Cache.GetAllEntityExtendedAttributesByEntityIdCacheKey(nameof(Document), x.EntityId))
                .ToListAsync(cancellationToken);
            cacheKeys.Add(ApplicationConstants.Cache.GetAllEntityExtendedAttributesCacheKey(nameof(Document)));
            cacheKeys.ForEach(s => _cache.Remove(s));
            return await base.Handle(command, cancellationToken);
        }
    }
}