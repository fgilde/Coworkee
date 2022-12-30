using System.Collections.Generic;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Enums;
using Coworkee.Shared.Wrapper;
using SDK;

namespace Coworkee.Client.Contracts;

public interface IDataTableAdapter<in TId, TDto> where TDto : IDtoBase<TId>
{
    Task<PaginatedResult<TDto>> LoadPagedAsync(int pageNumber, int pageSize, string searchString, string[] orderings);

    Task<List<TDto>> LoadAllAsync();

    Task<TDto> FindById(TId id, IEnumerable<TDto> alreadyLoaded);

    Task<Result> DeleteAsync(TId[] ids);

    string GetDisplayName(TDto dto);

    Task Export(ExportServiceType exportServiceType, string search);

    Task ExportSelected(ExportServiceType exportServiceType, TId[] ids);

    Task<bool> SaveAllAsync(TDto[] arg);

    Task<bool> CreateOrEditAsync(TDto itemOrNull);

    Task ImportAsync(FileParameter file);

}