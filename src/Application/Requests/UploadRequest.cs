using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using lib.Coworkee.Application.Contracts.Enums;
using Nextended.Core;
using Nextended.Core.Contracts;

namespace Coworkee.Application.Requests
{
    public class UploadRequest : IUploadableFile
    {
        public async Task EnsureDataLoadedAsync(HttpClient client = null)
        {
            if ((Data == null || Data?.Length == 0) && !string.IsNullOrEmpty(Url))
            {
                client ??= new HttpClient();
                Extension ??= System.IO.Path.GetExtension(Url);
                ContentType ??= await MimeType.ReadMimeTypeFromUrlAsync(Url, client);
                FileName ??= System.IO.Path.GetFileName(Url);
                Data = await client.GetByteArrayAsync(Url);
            }
        }
        
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public UploadType UploadType { get; set; }
        public byte[] Data { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }

        public static async Task<UploadRequest> FromUrlAsync(string url, CancellationToken cancellationToken = default) => new()
        {
            Extension = System.IO.Path.GetExtension(url),
            ContentType = await MimeType.ReadMimeTypeFromUrlAsync(url, cancellationToken),
            FileName = System.IO.Path.GetFileName(url),
            Url = url
        };
    }
}