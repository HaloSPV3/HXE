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
            Client = new HttpClient()
            { Timeout = defaultTimeout };
        }
        public static HttpClient Client { get; }
        public static readonly TimeSpan DEFAULT_TIMEOUT;
    }
}
