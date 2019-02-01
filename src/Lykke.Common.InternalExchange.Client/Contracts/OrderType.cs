using JetBrains.Annotations;

namespace Lykke.Common.InternalExchange.Client.Contracts
{
    /// <summary>
    /// Represents direction of the order.
    /// </summary>
    [PublicAPI]
    public enum OrderType
    {
        Buy,
        Sell
    }
}