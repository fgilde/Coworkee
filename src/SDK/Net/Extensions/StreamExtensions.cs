using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Coworkee.SDK.Net.Extensions;

public static class StreamExtensions
{
    public static async IAsyncEnumerable<string> AsChunkedUtf8Strings(this Task<Stream> streamTask, int chunkSize = 8192)
    {
        using var stream = await streamTask.ConfigureAwait(false);
        var buffer = new byte[chunkSize];
        var decoder = Encoding.UTF8.GetDecoder();
        var charBuffer = new char[chunkSize];

        while (true)
        {
            int bytesRead;
            try
            {
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                // Here comes network error
                break;
            }

            if (bytesRead == 0)
            {
                break;
            }

            int charsDecoded = decoder.GetChars(buffer, 0, bytesRead, charBuffer, 0);
            yield return new string(charBuffer, 0, charsDecoded);
        }
    }
}