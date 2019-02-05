using Lykke.Common.InternalExchange.Client.Api;
using Lykke.HttpClientGenerator;

namespace Lykke.Common.InternalExchange.Client
{
    /// <inheritdoc/>
    public class InternalExchangeClient : IInternalExchangeClient
    {
        /// <summary>
        /// Initializes a new instance of <see cref="InternalExchangeClient"/> with <param name="httpClientGenerator"></param>.
        /// </summary> 
        public InternalExchangeClient(IHttpClientGenerator httpClientGenerator)
        {
            InternalTrader = httpClientGenerator.Generate<IInternalTraderApi>();
        }

        /// <inheritdoc/>
        public IInternalTraderApi InternalTrader { get; }
    }
}