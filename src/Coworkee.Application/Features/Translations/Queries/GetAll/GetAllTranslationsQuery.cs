using System.Linq;
using System.Threading;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Repositories;
using Coworkee.Application.Features.Base.Queries;
using Coworkee.Domain.Entities.Localization;
using Coworkee.Shared.Constants.Application;
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