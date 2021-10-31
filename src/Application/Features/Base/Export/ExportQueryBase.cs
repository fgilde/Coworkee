using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Application.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Extensions;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Domain.Contracts;
using CleanArchitectureBase.Shared.Wrapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Base.Export
{
    public class ExportQueryBase<TId> : IRequest<Result<string>>
    {
        public string SearchString { get; set; }
        public TId[] Ids { get; }

        public ExportQueryBase(TId[] ids)
        {
            Ids = ids;
        }

        public ExportQueryBase(string searchString = "")
        {
            SearchString = searchString;
        }
    }

    internal abstract class ExportQueryHandlerBase<TQuery, TEntityId, TEntity> : IRequestHandler<TQuery, Result<string>>
        where TQuery : ExportQueryBase<TEntityId>
        where TEntity : AuditableEntity<TEntityId>
    {
        protected readonly IExcelService ExcelService;
        protected readonly IUnitOfWork<TEntityId> UnitOfWork;
        protected readonly IStringLocalizer Localizer;

        public ExportQueryHandlerBase(IExcelService excelService, IUnitOfWork<TEntityId> unitOfWork, IStringLocalizer localizer)
        {
            ExcelService = excelService;
            UnitOfWork = unitOfWork;
            Localizer = localizer;
        }

        protected abstract ISpecification<TEntity> GetFilterSpecification(TQuery query);

        protected virtual Dictionary<string, Func<TEntity, object>> PropertyMappers()
        {
            return typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(info => info.Name, info => new Func<TEntity, object>(e => info?.GetValue(e)));
        }

        public async Task<Result<string>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            var products = request.Ids is {Length: > 0} 
                ? await UnitOfWork.Repository<TEntity>().Entities.Where(p => request.Ids.Contains(p.Id)).ToListAsync(cancellationToken)
                : await UnitOfWork.Repository<TEntity>().Entities.Specify(GetFilterSpecification(request)).ToListAsync(cancellationToken);
            var data = await ExcelService.ExportAsync(products, PropertyMappers(), Localizer[typeof(TEntity).Name]);

            return await Result<string>.SuccessAsync(data: data);
        }
    }
}