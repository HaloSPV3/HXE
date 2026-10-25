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
            request.Version = HttpVersion.Version20;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
