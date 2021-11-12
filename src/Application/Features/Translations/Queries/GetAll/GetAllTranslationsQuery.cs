using System.Linq;
using System.Threading;
using CleanArchitectureBase.Application.Interfaces.Repositories;
using CleanArchitectureBase.Application.Dtos;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Localization;
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

        protected override string CacheKey (GetAllTranslationsQuery query)=> $"{base.CacheKey(query)}-{(query.FilterByCurrentCulture ? currentCulture: "all")}";

        protected override IQueryable<Translation> Query(GetAllTranslationsQuery query) => base.Query(query).Where(t => !query.FilterByCurrentCulture || t.CultureCode == currentCulture);
    }
}