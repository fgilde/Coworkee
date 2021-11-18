using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services.ExportImport;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace CleanArchitectureBase.Application.Features.Base.Export
{
    public class ExportQueryBase<TId> : IRequest<byte[]>
    {
        public ExportServiceType ExportServiceType { get; set; } = ExportServiceType.Excel;
        public string SearchString { get; set; }
        public TId[] Ids { get; set; }
    }

    internal abstract class ExportQueryHandlerBase<TQuery, TEntityId, TEntity> : IRequestHandler<TQuery, byte[]>
        where TQuery : ExportQueryBase<TEntityId>
        where TEntity : AuditableEntity<TEntityId>
    {
        protected readonly IUnitOfWork<TEntityId> UnitOfWork;
        protected readonly IStringLocalizer Localizer;
        protected readonly IServiceProvider Provider;
        protected T Get<T>() => Provider.GetService<T>();

        protected ExportQueryHandlerBase(IUnitOfWork<TEntityId> unitOfWork, IStringLocalizer localizer, IServiceProvider serviceProvider)
        {
            UnitOfWork = unitOfWork;
            Localizer = localizer;
            Provider = serviceProvider;
        }

        protected abstract ISpecification<TEntity> GetFilterSpecification(TQuery query);

        protected virtual Dictionary<string, Func<TEntity, object>> PropertyMappers()
        {
            return typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(info => info.Name, info => new Func<TEntity, object>(e => info?.GetValue(e)));
        }

        protected virtual IExportService GetExportService(TQuery query)
        {
            return Provider.GetServices<IExportService>()
                .FirstOrDefault(s => s.ExportService == query.ExportServiceType);
        }

        public async Task<byte[]> Handle(TQuery request, CancellationToken cancellationToken)
        {
            var service = GetExportService(request);
            var products = request.Ids is {Length: > 0} 
                ? await UnitOfWork.Repository<TEntity>().Entities.Where(p => request.Ids.Contains(p.Id)).ToListAsync(cancellationToken)
                : await UnitOfWork.Repository<TEntity>().Entities.Specify(GetFilterSpecification(request)).ToListAsync(cancellationToken);
            return await service.ExportAsync(products, PropertyMappers(), cancellationToken);
        }
    }
}