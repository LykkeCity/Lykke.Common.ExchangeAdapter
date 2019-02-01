using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.InternalExchange.Client.Contracts;
using Refit;

namespace Lykke.Common.InternalExchange.Client.Controllers
{
    /// <summary>
    /// Api for simulating trading between robots in Lykke.
    /// </summary>
    [PublicAPI]
    public interface IInternalTrader
    {
        /// <summary>
        /// Method to create order.
        /// </summary>
        /// <returns>Minimal information about order.</returns>
        [Post("/api/InternalTrader/Orders")]
        Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest model);
        
        /// <summary>
        /// Method to request order.
        /// </summary>
        /// <param name="orderId">Id of the order.</param>
        /// <returns>Information about order.</returns>
        [Get("/api/InternalTrader/Orders/{orderId}")]
        Task<OrderModel> GetOrderAsync(string orderId);
    }
}