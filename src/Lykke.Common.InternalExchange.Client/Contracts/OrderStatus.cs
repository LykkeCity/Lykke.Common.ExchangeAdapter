using JetBrains.Annotations;

namespace Lykke.Common.InternalExchange.Client.Contracts
{
    /// <summary>
    /// Represents state of the order.
    /// </summary>
    [PublicAPI]
    public enum OrderStatus
    {
        InProgress,
        Reject,
        Done
    }
}