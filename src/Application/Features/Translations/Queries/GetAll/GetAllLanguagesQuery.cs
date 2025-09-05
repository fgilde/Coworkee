using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Contracts.Repositories;
using lib.Coworkee.Application.Features.Base.Queries;
using lib.Coworkee.Domain.Entities.Localization;
using LazyCache;

namespace Coworkee.Application.Features.Translations.Queries.GetAll
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