using System.Linq;
using System.Threading;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Domain.Entities.Localization;
using lib.Coworkee.Shared.Constants.Application;
using LazyCache;

namespace Coworkee.Application.Features.Translations.Queries.GetAll
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