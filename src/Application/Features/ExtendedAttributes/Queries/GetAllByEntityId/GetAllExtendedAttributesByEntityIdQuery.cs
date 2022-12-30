using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Domain.Contracts;
using Coworkee.Shared.Constants.Application;
using Coworkee.Shared.Wrapper;
using LazyCache;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Nextended.Core.Extensions;

namespace Coworkee.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId
{
    public class GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequest<Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        public TEntityId EntityId { get; set; }

        public GetAllExtendedAttributesByEntityIdQuery(TEntityId entityId)
        {
            EntityId = entityId;
        }
    }

    internal class GetAllExtendedAttributesByEntityIdQueryHandler<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequestHandler<GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>, Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        private readonly IUnitOfWork<TId> _unitOfWork;
        private readonly IAppCache _cache;

        public GetAllExtendedAttributesByEntityIdQueryHandler(IUnitOfWork<TId> unitOfWork, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>> Handle(GetAllExtendedAttributesByEntityIdQuery<TId, TEntityId, TEntity, TExtendedAttribute> request, CancellationToken cancellationToken)
        {
            Task<List<TExtendedAttribute>> GetAllExtendedAttributesByEntityId() => _unitOfWork.Repository<TExtendedAttribute>().Entities.Where(x => x.EntityId.Equals(request.EntityId)).ToListAsync(cancellationToken);
            var extendedAttributeList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllEntityExtendedAttributesByEntityIdCacheKey(typeof(TEntity).Name, request.EntityId), GetAllExtendedAttributesByEntityId);
            var mappedExtendedAttributes = extendedAttributeList.MapTo<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>();
            return await Result<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>.SuccessAsync(mappedExtendedAttributes);
        }
    }
}