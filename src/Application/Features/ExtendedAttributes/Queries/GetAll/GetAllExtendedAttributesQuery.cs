using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Domain.Contracts;
using CleanArchitectureBase.Shared.Constants.Application;
using CleanArchitectureBase.Shared.Wrapper;
using LazyCache;
using MediatR;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.ExtendedAttributes.Queries.GetAll
{
    public class GetAllExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequest<Result<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        public GetAllExtendedAttributesQuery()
        {
        }
    }

    internal class GetAllExtendedAttributesQueryHandler<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequestHandler<GetAllExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>, Result<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
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