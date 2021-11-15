using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Responses.Audit;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Application.Contracts.Services
{
    public interface IAuditService
    {
        Task<IResult<IEnumerable<AuditResponse>>> GetCurrentUserTrailsAsync(string userId);

        Task<IResult<string>> ExportToExcelAsync(string userId, string searchString = "", bool searchInOldValues = false, bool searchInNewValues = false);
    }
}