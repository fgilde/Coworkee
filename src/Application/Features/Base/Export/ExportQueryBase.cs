using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Extensions;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Contracts.Services.ExportImport;
using CleanArchitectureBase.Application.Features.Base.Contracts;
using CleanArchitectureBase.Application.Specifications.Base;
using CleanArchitectureBase.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Nextended.Core.Extensions;

namespace CleanArchitectureBase.Application.Features.Base.Export
{
    public abstract class ExportQueryBase
    {
        public ExportServiceType ExportServiceType { get; set; } = ExportServiceType.Excel;
        public string SearchString { get; set; }
    }

    public class ExportQueryBase<TId> : ExportQueryBase ,IExportQuery<TId>
    {
        public TId[] Ids { get; set; }
    }

    public class ExportQueryHashed : ExportQueryBase, IExportQuery<int>
    {
        public string[] Ids { get; set; }
        int[] IExportQuery<int>.Ids => Ids?.Where(s=>!string.IsNullOrEmpty(s)).MapElementsTo<int>().ToArray();
    }

    internal abstract class ExportQueryHandlerBase<TQuery, TEntityId, TDto, TEntity> : IRequestHandler<TQuery, byte[]>
        where TQuery : IExportQuery<TEntityId>
        where TEntity : AuditableEntity<TEntityId>
        where TDto : class, IDtoBase
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

        protected virtual IExportService GetExportService(TQuery query)
        {
            return Provider.GetServices<IExportService>()
                .FirstOrDefault(s => s.ExportService == query.ExportServiceType);
        }

        public virtual async Task<byte[]> Handle(TQuery request, CancellationToken cancellationToken)
        {
            var service = GetExportService(request);
            var results = request.Ids is {Length: > 0} 
                ? await UnitOfWork.Repository<TEntity>().Entities.Where(p => request.Ids.Contains(p.Id)).ToListAsync(cancellationToken)
                : await UnitOfWork.Repository<TEntity>().Entities.Specify(GetFilterSpecification(request)).ToListAsync(cancellationToken);
            return await service.ExportAsync(results.MapElementsTo<TDto>(), cancellationToken);
        }
    }
}