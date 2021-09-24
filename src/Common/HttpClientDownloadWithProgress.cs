/// https://stackoverflow.com/a/43169927/14894786
using System.Threading.Tasks;
using System.IO;
using System;
using static HXE.Net.Http.GlobalHttpClient;
using System.Net.Http;

namespace HXE.Net.Http
{
    /// <summary>
    ///     A wrapper for a static HttpClient, with progress reporting.
    /// </summary>
    /// <remarks>
    ///     If you don't need to report download progress, use a plain, static HttpClient
    /// </remarks>
    public class HttpClientDownloadWithProgress
    {
        private readonly string _downloadUrl;
        private string _destinationFilePath;

        public Stream ContentStream;

        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);

        public event ProgressChangedHandler ProgressChanged;

        public HttpClientDownloadWithProgress(string downloadUrl, string destinationFilePath = "")
        {
            _downloadUrl = downloadUrl;
            _destinationFilePath = destinationFilePath;
        }

        /// <summary>
        ///     Start a download with the specified download URL
        /// </summary>
        /// <remarks>
        ///     If you want only the response header, <br/>
        ///     consider using `await StaticHttpClient.GetAsync(Uri, ResponseHeadersRead)` <br/>
        /// </remarks>
        public async Task StartDownload()
        {
            using (var response = await StaticHttpClient.GetAsync(_downloadUrl, HttpCompletionOption.ResponseHeadersRead))
                await DownloadFromHttpResponseMessage(response);
        }

        private async Task DownloadFromHttpResponseMessage(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();

            var totalBytes = response.Content.Headers.ContentLength;

            using (Stream contentStream = await response.Content.ReadAsStreamAsync())
                await ProcessContentStream(totalBytes, contentStream);
        }

        private async Task ProcessContentStream(long? totalDownloadSize, Stream contentStream)
        {
            var totalBytesRead = 0L;
            var readCount = 0L;
            var buffer = new byte[8192];
            var isMoreToRead = true;
            var largeContent = totalDownloadSize > 0x80000000; // 2 gibibytes
            var contentIsFile = !string.IsNullOrWhiteSpace(_destinationFilePath);

            /** If the content is large, but not a file, download to temp file */
            if (largeContent && !contentIsFile)
            {
                _destinationFilePath = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                contentIsFile = true;
            }

            using (FileStream fileStream = contentIsFile ?
                    new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true):
                    null
                    )
            {
                do
                {
                    var bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                        continue;
                    }

                    if (contentIsFile)
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                    /** else, access the stream variable */

                    totalBytesRead += bytesRead;
                    readCount++;

                    if (readCount % 100 == 0)
                        TriggerProgressChanged(totalDownloadSize, totalBytesRead);
                }
                while (isMoreToRead);
                if (!contentIsFile)
                    ContentStream = contentStream;
            }
        }

        private void TriggerProgressChanged(long? totalDownloadSize, long totalBytesRead)
        {
            if (ProgressChanged == null)
                return;

            double? progressPercentage = null;
            if (totalDownloadSize.HasValue)
                progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);

            ProgressChanged(totalDownloadSize, totalBytesRead, progressPercentage);
        }
    }
}
