using JetBrains.Annotations;

namespace Lykke.Common.InternalExchange.Client.Models
{
    /// <summary>
    /// Specifies order status.
    /// </summary>
    [PublicAPI]
    public enum OrderStatus
    {
        /// <summary>
        /// Unspecified order status.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the order currently in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// Indicated that the order rejected. See rejection reason for more details.
        /// </summary>
        Reject,

        /// <summary>
        /// Indicates that order was processed successfully.
        /// </summary>
        Done
    }
}