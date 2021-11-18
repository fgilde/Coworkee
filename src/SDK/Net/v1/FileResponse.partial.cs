using System;
using System.IO;
using System.Net.Http;

namespace SDK
{
    public partial class FileResponse
    {
        private string _base64;
        public bool IsSuccessStatusCode => StatusCode is >= 200 and <= 299;
        public HttpResponseMessage Response => _response as HttpResponseMessage;
        public string MimeType => Response?.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        public string FileName => Response?.Content.Headers.ContentDisposition?.FileName;
        public string Base64String => _base64 ??= AsBase64String();
        public object JsDownloadObject => new {Base64String, FileName, MimeType};

        private string AsBase64String()
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                Stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);
        }
    }
}