using System;
using System.Net.Http;

namespace HXE.Net.Http
{
    public static class GlobalHttpClient
    {
        public static HttpClient StaticHttpClient { get; } = new HttpClient { Timeout = DEFAULT_TIMEOUT };
        public static readonly TimeSpan DEFAULT_TIMEOUT = TimeSpan.FromDays(1);
    }
}
