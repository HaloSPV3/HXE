/// https://stackoverflow.com/a/43169927/14894786
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Buffers;

namespace System.Net.Http
{
    public delegate void DownloadProgressHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);

    public static class DownloadWithProgress
    {
        /// <summary>
        ///     An amalgamation of StackOverflow Answers
        /// </summary>
        /// <remarks>
        ///     EXAMPLE:                                                                        <br/>
        ///     ```cs                                                                           <br/>
        ///     await DownloadWithProgress.ExecuteAsync(                                        <br/>
        ///            HttpClients.General, assetUrl, downloadFilePath, progressHandler, () =>  <br/>
        ///     {                                                                               <br/>
        ///         var requestMessage = new HttpRequestMessage(HttpMethod.Get, assetUrl);      <br/>
        ///         requestMessage.Headers.Accept.TryParseAdd("application/octet-stream");      <br/>
        ///         return requestMessage;                                                      <br/>
        ///     });                                                                             <br/>
        ///     ```
        /// </remarks>
        public static async Task ExecuteAsync(HttpClient httpClient, string downloadPath, string destinationPath, DownloadProgressHandler progress, Func<HttpRequestMessage> requestMessageBuilder = null, CancellationToken? cancellationToken = null)
        {
            requestMessageBuilder ??= GetDefaultRequestBuilder(downloadPath);
            var download = new HttpClientDownloadWithProgress(httpClient, destinationPath, requestMessageBuilder, cancellationToken);
            download.ProgressChanged += progress;
            await download.StartDownload();
            download.ProgressChanged -= progress;
        }

        private static Func<HttpRequestMessage> GetDefaultRequestBuilder(string downloadPath)
        {
            return () => new HttpRequestMessage(HttpMethod.Get, downloadPath);
        }
    }

    internal class HttpClientDownloadWithProgress
    {
        private readonly string _destinationFilePath;
        private readonly CancellationToken? _cancellationToken;

        private readonly HttpClient _httpClient;

        public delegate void ProgressChangedHandler(long? totalFileSize, long totalBytesDownloaded, double? progressPercentage);

        public event DownloadProgressHandler ProgressChanged;

        private readonly Func<HttpRequestMessage> _requestMessageBuilder;
        private readonly int _bufferSize = 8192;

        public HttpClientDownloadWithProgress(HttpClient httpClient, string destinationFilePath, Func<HttpRequestMessage> requestMessageBuilder, CancellationToken? cancellationToken)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _destinationFilePath = destinationFilePath ?? throw new ArgumentNullException(nameof(destinationFilePath));
            _requestMessageBuilder = requestMessageBuilder ?? throw new ArgumentNullException(nameof(requestMessageBuilder));
            _cancellationToken = cancellationToken;
        }

        /// <summary>
        /// TODO. SendAsync is way more complex than GetAsync. I don'tunderstand it at all.
        /// </summary>
        public async Task StartDownload()
        {
            using (var requestMessage = _requestMessageBuilder.Invoke())
            using (var response = await _httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead))
                await DownloadAsync(response);
        }

        private async Task DownloadAsync(HttpResponseMessage response)
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
            var buffer = ArrayPool<byte>.Shared.Rent(_bufferSize);
            var isMoreToRead = true;

            using (var fileStream = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, _bufferSize, true))
            {
                do
                {
                    int bytesRead;
                    if (_cancellationToken.HasValue)
                    {
                        bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, _cancellationToken.Value);
                    }
                    else
                    {
                        bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length);
                    }

                    if (bytesRead == 0)
                    {
                        isMoreToRead = false;
                        ReportProgress(totalDownloadSize, totalBytesRead);
                        continue;
                    }

                    await fileStream.WriteAsync(buffer, 0, bytesRead);

                    totalBytesRead += bytesRead;
                    readCount += 1;

                    if (readCount % 100 == 0)
                        ReportProgress(totalDownloadSize, totalBytesRead);
                }
                while (isMoreToRead);

            }

            //the last progress trigger should occur after the file handle has been released or you may get file locked error
            ReportProgress(totalDownloadSize, totalBytesRead);
            ArrayPool<byte>.Shared.Return(buffer);
        }

        private void ReportProgress(long? totalDownloadSize, long totalBytesRead)
        {
            if (ProgressChanged == null)
                return;

            double? progressPercentage = null;
            if (totalDownloadSize.HasValue)
                progressPercentage = Math.Round((double)totalBytesRead / totalDownloadSize.Value * 100, 2);

            ProgressChanged?.Invoke(totalDownloadSize, totalBytesRead, progressPercentage);
        }
    }
}
