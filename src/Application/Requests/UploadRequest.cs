using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Coworkee.Application.Contracts.Enums;
using Nextended.Core;
using Nextended.Core.Contracts;

namespace Coworkee.Application.Requests
{
    public class UploadRequest : IUploadableFile
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public UploadType UploadType { get; set; }
        public byte[] Data { get; set; }
        public string Url { get; set; }
        public static async Task<UploadRequest> FromUrlAsync(string url, CancellationToken cancellationToken = default) => new()
        {
            Extension = Path.GetExtension(url),
            ContentType = await MimeType.ReadMimeTypeFromUrlAsync(url, cancellationToken),
            FileName = Path.GetFileName(url),
            Url = url
        };
    }
}