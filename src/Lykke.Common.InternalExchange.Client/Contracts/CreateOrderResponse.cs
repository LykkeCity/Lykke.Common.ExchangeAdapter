using JetBrains.Annotations;

namespace Lykke.Common.InternalExchange.Client.Contracts
{
    /// <summary>
    /// Represents result of order creation.
    /// </summary>
    [PublicAPI]
    public class CreateOrderResponse
    {
        /// <summary>
        /// Id of the created order.
        /// </summary>
        public string OrderId { set; get; }
    }
}