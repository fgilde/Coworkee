using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Responses.Audit;
using CleanArchitectureBase.Shared.Wrapper;

namespace CleanArchitectureBase.Application.Contracts.Services
{
    public interface IAuditService
    {
        Task<IResult<IEnumerable<AuditResponse>>> GetTrailsAsync(int limit = 1000, params string[] userIds);

        Task<IResult<string>> ExportAsync(string[] userId, 
                string searchString = "", 
                bool searchInOldValues = false, 
                bool searchInNewValues = false);
    }
}