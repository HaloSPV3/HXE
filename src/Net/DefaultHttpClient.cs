using System;
using System.Net.Http;

namespace HXE.Net
{
    public static class DefaultHttpClient
    {
        public static HttpClient Client { get; } = new HttpClient { Timeout = DEFAULT_TIMEOUT };
        public static readonly TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromDays(1);
    }
}
