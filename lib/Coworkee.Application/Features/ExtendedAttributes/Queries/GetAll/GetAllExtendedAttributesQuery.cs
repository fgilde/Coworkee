using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Domain.Contracts;
using lib.Coworkee.Shared.Constants.Application;
using lib.Coworkee.Shared.Wrapper;
using LazyCache;
using MediatR;
using Nextended.Core.Extensions;

namespace lib.Coworkee.Application.Features.ExtendedAttributes.Queries.GetAll
{
    public class GetAllExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequest<Result<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>>
            where TEntity : class, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        public GetAllExtendedAttributesQuery()
        {
        }
    }

    internal class GetAllExtendedAttributesQueryHandler<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequestHandler<GetAllExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>, Result<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>>
            where TEntity : class, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        private readonly IUnitOfWork<TId> _unitOfWork;
        private readonly IAppCache _cache;

        public GetAllExtendedAttributesQueryHandler(IUnitOfWork<TId> unitOfWork, IAppCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task<Result<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>> Handle(GetAllExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute> request, CancellationToken cancellationToken)
        {
            Task<List<TExtendedAttribute>> GetAllExtendedAttributes() => _unitOfWork.Repository<TExtendedAttribute>().GetAllAsync();
            var extendedAttributeList = await _cache.GetOrAddAsync(ApplicationConstants.Cache.GetAllEntityExtendedAttributesCacheKey(typeof(TEntity).Name), GetAllExtendedAttributes);
            var mappedExtendedAttributes = extendedAttributeList.MapTo<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>();
            return await Result<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>.SuccessAsync(mappedExtendedAttributes);
        }
    }
}