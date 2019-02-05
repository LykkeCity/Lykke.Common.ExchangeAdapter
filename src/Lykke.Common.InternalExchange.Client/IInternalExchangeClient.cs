using JetBrains.Annotations;
using Lykke.Common.InternalExchange.Client.Api;

namespace Lykke.Common.InternalExchange.Client
{
    /// <summary>
    /// Internal exchange service client.
    /// </summary>
    [PublicAPI]
    public interface IInternalExchangeClient
    {
        /// <summary>
        /// Internal trader API.
        /// </summary>
        IInternalTraderApi InternalTrader { get; }
    }
}