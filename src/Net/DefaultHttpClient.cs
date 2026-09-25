using System;
using System.Net.Http;

namespace HXE.Net
{
    public static class DefaultHttpClient
    {
        static DefaultHttpClient()
        {
            // the runtime does not allow reading a field/property before exiting the constructor;
            // (The resulting OutOfRangeException was misleading and took a few evenings to debug)
            // To instantiate-once-assign-twice, one must assign the instance to a temporary variable
            // and use that variable for the other assignments.
            var defaultTimeout = TimeSpan.FromSeconds(30);
            DEFAULT_TIMEOUT = defaultTimeout;
            var handler = new HttpClientHandler()
            {
                AutomaticDecompression = 
#if NET5_0_OR_GREATER
                    System.Net.DecompressionMethods.All,
#elif NET462_OR_GREATER
                    System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate,
#endif
            };
            Client = new HttpClient(handler)
            { Timeout = defaultTimeout };
        }
        public static HttpClient Client { get; }
        public static readonly TimeSpan DEFAULT_TIMEOUT;
    }
}
