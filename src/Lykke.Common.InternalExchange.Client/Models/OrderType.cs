using JetBrains.Annotations;

namespace Lykke.Common.InternalExchange.Client.Models
{
    /// <summary>
    /// Specifies order type.
    /// </summary>
    [PublicAPI]
    public enum OrderType
    {
        /// <summary>
        /// Unspecified order type.
        /// </summary>
        None,

        /// <summary>
        /// Buy order type.
        /// </summary>
        Buy,

        /// <summary>
        /// Sell order type.
        /// </summary>
        Sell
    }
}