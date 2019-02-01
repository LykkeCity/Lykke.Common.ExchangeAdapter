using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.InternalExchange.Client.Models;
using Refit;

namespace Lykke.Common.InternalExchange.Client.Api
{
    /// <summary>
    /// Provides methods for internal trading.
    /// </summary>
    [PublicAPI]
    public interface IInternalTraderApi
    {
        /// <summary>
        /// Creates order.
        /// </summary>
        /// <returns>The result of order creation.</returns>
        [Post("/api/InternalTrader/Orders")]
        Task<CreateOrderResponse> CreateOrderAsync([Body] CreateOrderRequest model);

        /// <summary>
        /// Returns order by identifier.
        /// </summary>
        /// <param name="orderId">The identifier of the order.</param>
        /// <returns>Information about order.</returns>
        [Get("/api/InternalTrader/Orders/{orderId}")]
        Task<OrderModel> GetOrderAsync(string orderId);
    }
}