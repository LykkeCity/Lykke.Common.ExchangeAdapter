using JetBrains.Annotations;

namespace Lykke.Common.InternalExchange.Client.Contracts
{
    /// <summary>
    /// Represents request.
    /// </summary>
    [PublicAPI]
    public class GetOrderRequest
    {
        /// <summary>
        /// Id of the order.
        /// </summary>
        public string OrderId { set; get; }
    }
}