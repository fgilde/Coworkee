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
    {}

    internal class GetAllTranslationsQueryHandler : GetAllQueryHandlerBase<GetAllTranslationsQuery, int, TranslationDto, Translation>
    {
        public GetAllTranslationsQueryHandler(IUnitOfWork<int> unitOfWork, IAppCache cache) : base(unitOfWork, cache)
        { }

        private string currentCulture => Thread.CurrentThread.CurrentCulture.Name;

        protected override string CacheKey => $"{base.CacheKey}-{currentCulture}";

        protected override IQueryable<Translation> Queryable => base.Queryable.Where(t => t.CultureCode == currentCulture);
    }
}