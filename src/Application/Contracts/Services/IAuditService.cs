using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitectureBase.Application.Contracts.Enums;
using CleanArchitectureBase.Application.Responses.Audit;

namespace CleanArchitectureBase.Application.Contracts.Services
{
    public interface IAuditService
    {
        Task<IReadOnlyCollection<AuditResponse>> GetTrailsAsync(int limit = 1000, params string[] userIds);

        Task<byte[]> ExportAsync(
                ExportServiceType exportServiceType,
                string[] userId, 
                string searchString = "", 
                bool searchInOldValues = false, 
                bool searchInNewValues = false,
                CancellationToken cancellationToken = default);
    }
}