using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Common.ExchangeAdapter.Client
{
    public class XApiKeyHandler : DelegatingHandler
    {
        private readonly string _apiKey;

        /// <inheritdoc />
        public XApiKeyHandler(string apiKey)
        {
            _apiKey = apiKey;
        }

        /// <inheritdoc />
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            request.Headers.TryAddWithoutValidation("X-API-KEY", _apiKey);
            return base.SendAsync(request, cancellationToken);
        }
    }
}