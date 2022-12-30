using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Contracts.Enums;

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