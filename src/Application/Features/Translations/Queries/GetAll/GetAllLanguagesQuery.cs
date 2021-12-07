using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Repositories;
using CleanArchitectureBase.Application.Features.Base.Queries;
using CleanArchitectureBase.Domain.Entities.Localization;
using LazyCache;

namespace CleanArchitectureBase.Application.Features.Translations.Queries.GetAll
{
    public class GetAllLanguagesQuery : GetAllQueryBase<LanguageDto>
    {}

    internal class GetAllLanguagesQueryHandler : GetAllQueryHandlerBase<GetAllLanguagesQuery, int, LanguageDto, Language>
    {
        protected override string CacheKey(GetAllLanguagesQuery query) => null;

        public GetAllLanguagesQueryHandler(IUnitOfWork<int> unitOfWork, IAppCache cache) : base(unitOfWork, cache)
        { }
    }
}