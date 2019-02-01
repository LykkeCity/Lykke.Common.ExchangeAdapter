using JetBrains.Annotations;

namespace Lykke.Common.InternalExchange.Client.Contracts
{
    /// <summary>
    /// Represents order creation request.
    /// </summary>
    [PublicAPI]
    public class CreateOrderRequest
    {
        /// <summary>
        /// WalletId with which robot should trade.
        /// </summary>
        public string WalletId { set; get; }
        
        /// <summary>
        /// Id of the asset pair.
        /// </summary>
        public string AssetPair { set; get; }
        
        /// <summary>
        /// Direction of the order.
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
        /// True if only full execution is allowed.
        /// </summary>
        public bool FullExecution { set; get; }
    }
}