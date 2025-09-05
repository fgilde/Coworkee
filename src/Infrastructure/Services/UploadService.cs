using System.Collections.Generic;
using lib.Coworkee.Application.Requests;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Services;
using Nextended.Core.Attributes;
using Nextended.Core.Helper;

namespace Coworkee.Infrastructure.Services
{
    [RegisterAs(typeof(IUploadService), 7)]
    public class UploadService : IUploadService
    {
        private readonly IFileAccess _fileAccess;

        public UploadService(IFileAccess fileAccess)
        {
            _fileAccess = fileAccess;
        }

        public Task<string> UploadAsync(UploadRequest request, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Upload(request), cancellationToken);
        }

        public string[] Upload(IEnumerable<UploadRequest> requests)
        {
            return requests.Select(Upload).ToArray();
        }

        public Task<string[]> UploadAsync(IEnumerable<UploadRequest> requests, CancellationToken cancellationToken = default)
        {
            return Task.WhenAll(requests.Select(r => UploadAsync(r, cancellationToken)));
        }

        public string Upload(UploadRequest request)
        {
            if (!string.IsNullOrEmpty(request.Url))
            {
                // TODO Download to...
                return request.Url;
            }

            if (request.Data == null) return string.Empty;
            var streamData = new MemoryStream(request.Data);
            if (streamData.Length > 0)
            {

                var fullPath = _fileAccess.EnsureFileNotExists(request.UploadType.ToDescriptionString(), request.FileName.Trim('"'));

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    streamData.CopyTo(stream);
                }
                return _fileAccess.GetRelativeUrl(fullPath);
            }

            return string.Empty;
        }

    }
}