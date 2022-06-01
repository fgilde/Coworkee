using System.Linq;
using System.Threading;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Localization;
using CleanArchitectureBase.Shared.Constants.Application;
using LazyCache;

namespace CleanArchitectureBase.Application.Features.Translations.Queries.GetAll
{
    public class GetAllTranslationsQuery : GetAllQueryBase<TranslationDto>
    {
        public bool FilterByCurrentCulture { get; set; } = true;
    }

    internal class GetAllTranslationsQueryHandler : GetAllQueryHandlerBase<GetAllTranslationsQuery, int, TranslationDto, Translation>
    {
        public GetAllTranslationsQueryHandler(IUnitOfWork<int> unitOfWork, IAppCache cache) : base(unitOfWork, cache)
        { }

        private string currentCulture => Thread.CurrentThread.CurrentCulture.Name;

        protected override string CacheKey(GetAllTranslationsQuery query)
        {
            if (query.FilterByCurrentCulture)
                return ApplicationConstants.Cache.CacheKeyFor(typeof(Translation), query.OdataFilterQuery, currentCulture);
            return ApplicationConstants.Cache.CacheKeyFor(typeof(Translation), query.OdataFilterQuery);
        }

        protected override IQueryable<Translation> Query(GetAllTranslationsQuery query, IQueryable<Translation> entities)
            => base.Query(query, entities).Where(t => !query.FilterByCurrentCulture || t.CultureCode == currentCulture);
    }
}