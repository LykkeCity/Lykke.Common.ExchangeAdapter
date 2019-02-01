using JetBrains.Annotations;

namespace Lykke.Common.InternalExchange.Client.Models
{
    /// <summary>
    /// Represents result of order creation.
    /// </summary>
    [PublicAPI]
    public class CreateOrderResponse
    {
        /// <summary>
        /// The identifier of the created order.
        /// </summary>
        public string OrderId { set; get; }
    }
}