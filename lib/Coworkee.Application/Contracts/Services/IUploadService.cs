using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Requests;

namespace Coworkee.Application.Contracts.Services
{
    public interface IUploadService
    {
        string Upload(UploadRequest request);
        Task<string> UploadAsync(UploadRequest request, CancellationToken cancellationToken = default);
        string[] Upload(IEnumerable<UploadRequest> requests);
        Task<string[]> UploadAsync(IEnumerable<UploadRequest> requests, CancellationToken cancellationToken = default);
    }
}