using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Extensions;
using lib.Coworkee.Application.Contracts.Enums;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Contracts.Services.ExportImport;
using lib.Coworkee.Application.Specifications.ExtendedAttribute;
using lib.Coworkee.Domain.Contracts;
using lib.Coworkee.Domain.Enums;
using lib.Coworkee.Shared.Wrapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace lib.Coworkee.Application.Features.ExtendedAttributes.Queries.Export
{
    internal class ExportExtendedAttributesQueryLocalization
    {
        // for localization
    }

    public class ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequest<Result<string>>
            where TEntity : class, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        public string SearchString { get; set; }
        public TEntityId EntityId { get; set; }
        public bool IncludeEntity { get; set; }
        public bool OnlyCurrentGroup { get; set; }
        public string CurrentGroup { get; set; }

        public ExportExtendedAttributesQuery(string searchString = "", TEntityId entityId = default, bool includeEntity = false, bool onlyCurrentGroup = false, string currentGroup = "")
        {
            SearchString = searchString;
            EntityId = entityId;
            IncludeEntity = includeEntity;
            OnlyCurrentGroup = onlyCurrentGroup;
            CurrentGroup = currentGroup;
        }
    }

    internal class ExportExtendedAttributesQueryHandler<TId, TEntityId, TEntity, TExtendedAttribute>
        : IRequestHandler<ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute>, Result<string>>
            where TEntity : class, IEntityWithExtendedAttributes<TExtendedAttribute>, IEntity<TEntityId>
            where TExtendedAttribute : AuditableEntityExtendedAttribute<TId, TEntityId, TEntity>, IEntity<TId>
            where TId : IEquatable<TId>
    {
        private readonly IExportService _excelService;
        private readonly IUnitOfWork<TId> _unitOfWork;
        private readonly IStringLocalizer<ExportExtendedAttributesQueryLocalization> _localizer;

        public ExportExtendedAttributesQueryHandler(IServiceProvider serviceProvider
            , IUnitOfWork<TId> unitOfWork
            , IStringLocalizer<ExportExtendedAttributesQueryLocalization> localizer)
        {
            _excelService = serviceProvider.GetServices<IExportService>().FirstOrDefault(s => s.ExportService == ExportServiceType.Excel);
            _unitOfWork = unitOfWork;
            _localizer = localizer;
        }

        public async Task<Result<string>> Handle(ExportExtendedAttributesQuery<TId, TEntityId, TEntity, TExtendedAttribute> request, CancellationToken cancellationToken)
        {
            var extendedAttributeFilterSpec = new ExtendedAttributeFilterSpecification<TId, TEntityId, TEntity, TExtendedAttribute>(request);
            var extendedAttributes = await _unitOfWork.Repository<TExtendedAttribute>().Entities
                .Specify(extendedAttributeFilterSpec)
                .ToListAsync(cancellationToken);

            // check SearchString outside of specification because of
            // an expression tree lambda may not contain a null propagating operator
            if (!string.IsNullOrWhiteSpace(request.SearchString))
            {
                extendedAttributes = extendedAttributes.Where(p =>
                        p.Key.Contains(request.SearchString, StringComparison.InvariantCultureIgnoreCase)
                        || p.Decimal?.ToString().Contains(request.SearchString, StringComparison.InvariantCultureIgnoreCase) == true
                        || p.Text?.Contains(request.SearchString, StringComparison.InvariantCultureIgnoreCase) == true
                        || p.DateTime?.ToString("G", CultureInfo.CurrentCulture).Contains(request.SearchString, StringComparison.InvariantCultureIgnoreCase) == true
                        || p.Json?.Contains(request.SearchString, StringComparison.InvariantCultureIgnoreCase) == true
                        || p.ExternalId?.Contains(request.SearchString, StringComparison.InvariantCultureIgnoreCase) == true
                        || p.Description?.Contains(request.SearchString, StringComparison.InvariantCultureIgnoreCase) == true
                        || p.Group?.Contains(request.SearchString, StringComparison.InvariantCultureIgnoreCase) == true)
                    .ToList();
            }


            var data = await _excelService.ExportAsync(extendedAttributes, cancellationToken);
            return await Result<string>.SuccessAsync(data: Convert.ToBase64String(data));
        }
    }
}