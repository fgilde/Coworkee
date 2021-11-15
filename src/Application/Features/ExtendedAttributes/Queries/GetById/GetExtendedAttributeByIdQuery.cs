using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Domain.Contracts;
using CleanArchitectureBase.Shared.Wrapper;
using MediatR;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.ExtendedAttributes.Queries.GetById
{
    public class GetExtendedAttributeByIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequest<Result<GetExtendedAttributeByIdResponse<TId, TEntityId>>>
        where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
        where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
        where TId : IEquatable<TId>
    {
        public TId Id { get; set; }
    }

    internal class GetExtendedAttributeByIdQueryHandler<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequestHandler<GetExtendedAttributeByIdQuery<TId, TEntityId, TEntity, TExtendedAttribute>, Result<GetExtendedAttributeByIdResponse<TId, TEntityId>>>
            where TEntity : AuditableEntity<TEntityId>, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        private readonly IUnitOfWork<TId> _unitOfWork;

        public GetExtendedAttributeByIdQueryHandler(IUnitOfWork<TId> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<GetExtendedAttributeByIdResponse<TId, TEntityId>>> Handle(GetExtendedAttributeByIdQuery<TId, TEntityId, TEntity, TExtendedAttribute> query, CancellationToken cancellationToken)
        {
            var extendedAttribute = await _unitOfWork.Repository<TExtendedAttribute>().GetByIdAsync(query.Id);
            var mappedExtendedAttribute = extendedAttribute.MapTo<GetExtendedAttributeByIdResponse<TId, TEntityId>>();
            return await Result<GetExtendedAttributeByIdResponse<TId, TEntityId>>.SuccessAsync(mappedExtendedAttribute);
        }
    }
}