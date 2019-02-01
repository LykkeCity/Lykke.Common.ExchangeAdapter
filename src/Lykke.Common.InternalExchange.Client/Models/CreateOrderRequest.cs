using JetBrains.Annotations;

namespace Lykke.Common.InternalExchange.Client.Models
{
    /// <summary>
    /// Represents order creation request.
    /// </summary>
    [PublicAPI]
    public class CreateOrderRequest
    {
        /// <summary>
        /// The identifier of te wallet that should be used to transfer funds.
        /// </summary>
        public string WalletId { set; get; }

        /// <summary>
        /// The identifier of the asset pair.
        /// </summary>
        public string AssetPair { set; get; }

        /// <summary>
        /// The type of order.
        /// </summary>
        public OrderType Type { set; get; }

        /// <summary>
        /// Desired price of execution.
        /// </summary>
        public decimal Price { set; get; }

        /// <summary>
        /// Desired volume of execution.
        /// </summary>
        public decimal Volume { set; get; }

        /// <summary>
        /// If <c>true</c> the order should be fully executed; otherwise, part execution allowed.
        /// </summary>
        public bool FullExecution { set; get; }
    }
}