using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Common.Models;
using lib.Coworkee.Application.Contracts.Enums;

namespace Coworkee.Application.Contracts.Services
{
    public interface IAuditService
    {
        Task<IReadOnlyCollection<AuditDto>> GetTrailsAsync(int limit = 1000, params string[] userIds);

        Task<byte[]> ExportAsync(
                ExportServiceType exportServiceType,
                string[] userId, 
                string searchString = "", 
                bool searchInOldValues = false, 
                bool searchInNewValues = false,
                CancellationToken cancellationToken = default);
    }
}