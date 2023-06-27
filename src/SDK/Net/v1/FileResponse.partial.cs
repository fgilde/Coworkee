using SendGrid;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Coworkee.SDK.Net.Extensions;
using System.Threading;

namespace SDK
{
    public partial class FileResponse
    {
        private string _base64;
        public bool IsSuccessStatusCode => StatusCode is >= 200 and <= 299;
        public HttpResponseMessage Response => _response as HttpResponseMessage;
        public string MimeType => Response?.Content.Headers.ContentType?.MediaType ?? "application/octet-stream";
        public string FileName => Response?.Content.Headers.ContentDisposition?.FileName ?? $"File-{DateTime.Now:ddMMyyyyHHmmss}";
        public string Base64String => _base64 ??= AsBase64String();
        public object JsDownloadObject => new {Base64String, FileName, MimeType};

        public async IAsyncEnumerable<string> ReadAsStringStreamAsync(int chunkSize = 8192, CancellationToken cancellationToken = default )
        {
            //var responseStream = await Response.Content.ReadAsStreamAsync(); // Stream
            //using var streamReader = new StreamReader(responseStream, Encoding.UTF8);

            //while (!streamReader.EndOfStream)
            //{
            //    string receivedString = await streamReader.ReadLineAsync();
            //    yield return receivedString;
            //}

            Response.EnsureSuccessStatusCode();
            await foreach (string chunk in Response.Content.ReadAsStreamAsync(cancellationToken).AsChunkedUtf8Strings(chunkSize).WithCancellation(cancellationToken))
            {
                yield return chunk;
            }
        }

        public async Task ReadAsStringStreamAsync(Action<string> onReceivedString, int chunkSize = 8192, CancellationToken cancellationToken = default)
        {
            await foreach (var receivedString in ReadAsStringStreamAsync(chunkSize, cancellationToken))
            {
                onReceivedString(receivedString);
            }
        }

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