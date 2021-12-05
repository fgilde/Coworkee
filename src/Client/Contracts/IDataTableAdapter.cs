using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Common.Models;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Shared.Wrapper;
using SDK;

namespace CleanArchitectureBase.Client.Contracts;

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