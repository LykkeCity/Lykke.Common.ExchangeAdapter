using JetBrains.Annotations;
using Lykke.Common.ExchangeAdapter.SpotController;

namespace Lykke.Common.ExchangeAdapter.Client
{
    /// <summary>
    /// Provides methods to get exchange adapter clients.
    /// </summary>
    [PublicAPI]
    public interface IExchangeAdapterClientFactory
    {
        /// <summary>
        /// Returns spot API client for provided adapter.
        /// </summary>
        /// <param name="adapter">The name of adapter.</param>
        /// <returns>The client for spot API.</returns>
        ISpotController GetSpotController(string adapter);

        /// <summary>
        /// Returns order book API client for provided adapter.
        /// </summary>
        /// <param name="adapter">The name of adapter.</param>
        /// <returns>The client for order book API.</returns>
        IOrderBookController GetOrderBookController(string adapter);
    }
}