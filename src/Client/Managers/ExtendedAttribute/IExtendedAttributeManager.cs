using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using lib.Coworkee.Application.Features.ExtendedAttributes.Commands.AddEdit;
using lib.Coworkee.Application.Features.ExtendedAttributes.Queries.Export;
using lib.Coworkee.Application.Features.ExtendedAttributes.Queries.GetAll;
using lib.Coworkee.Application.Features.ExtendedAttributes.Queries.GetAllByEntityId;
using lib.Coworkee.Domain.Contracts;
using lib.Coworkee.Shared.Wrapper;

namespace Coworkee.Client.Managers.ExtendedAttribute
{
    public interface IExtendedAttributeManager<TId, TEntityId, TEntity, TExtendedAttribute>
        where TEntity : class, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
        where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
        where TId : IEquatable<TId>
    {
        Task<IResult<List<GetAllExtendedAttributesResponse<TId, TEntityId>>>> GetAllAsync();

        Task<IResult<List<GetAllExtendedAttributesByEntityIdResponse<TId, TEntityId>>>> GetAllByEntityIdAsync(TEntityId entityId);

        Task<IResult<TId>> SaveAsync(AddEditExtendedAttributeCommand<TId, TEntityId, TEntity, TExtendedAttribute> request);

        Task<IResult<TId>> DeleteAsync(TId id);

        Task<IResult<string>> ExportToExcelAsync(ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute> request);
    }
}