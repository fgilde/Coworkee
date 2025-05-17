using System;
using System.Linq;
using Coworkee.Domain.Entities.Misc;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Common.Security;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Contracts.Services;
using Coworkee.Application.Features.Base.Commands;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Constants.Permission;
using LazyCache;
using Microsoft.EntityFrameworkCore;

namespace Coworkee.Application.Features.Documents.Commands.Delete;

[CustomAuthorize(Policies = new[] { Permissions.Documents.Delete })]
public class DeleteDocumentsCommand : DeleteCommandBase<int>
{
    public bool DeleteFiles { get; set; } = true;
}

internal class DeleteDocumentsCommandHandler : DeleteCommandHandlerBase<DeleteDocumentsCommand, int, DocumentDto, Document>
{
    private readonly IAppCache _cache;

    public DeleteDocumentsCommandHandler(IUnitOfWork<int> unitOfWork, IMediator mediator, IPermissionService permissionService, IServiceProvider provider, IAppCache cache)
        : base(unitOfWork, mediator, permissionService, provider)
    {
        _cache = cache;
    }

    public override async Task Handle(DeleteDocumentsCommand command, CancellationToken cancellationToken)
    {
        var documentsWithExtendedAttributes = UnitOfWork.Repository<Document>().Entities.Include(x => x.ExtendedAttributes);
        if (command.DeleteFiles)
        {
            var fileAccess = Get<IFileAccess>();
            var tasks = documentsWithExtendedAttributes
                .Where(x => command.Ids.Contains(x.Id) && !string.IsNullOrEmpty(x.URL))
                .Select(x => fileAccess.DeleteAsync(x.URL));
            await Task.WhenAll(tasks);
        }
        var cacheKeys = await documentsWithExtendedAttributes.SelectMany(x => x.ExtendedAttributes).Where(x => command.Ids.Contains(x.EntityId))
            .Distinct().Select(x => ApplicationConstants.Cache.GetAllEntityExtendedAttributesByEntityIdCacheKey(nameof(Document), x.EntityId))
            .ToListAsync(cancellationToken);
        cacheKeys.Add(ApplicationConstants.Cache.GetAllEntityExtendedAttributesCacheKey(nameof(Document)));
        cacheKeys.ForEach(s => _cache.Remove(s));
        await base.Handle(command, cancellationToken);
    }
}