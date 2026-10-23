using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HXE.Net
{
    internal class ForceHttp2Handler(HttpMessageHandler innerHandler) : DelegatingHandler(innerHandler)
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Version =
#if NET5_0_OR_GREATER
            HttpVersion.Version20;
#else
            new System.Version("2.0");
#endif
            return base.SendAsync(request, cancellationToken);
        }
    }
}
