using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.SpotController.Records;
using Refit;

namespace Lykke.Common.ExchangeAdapter.SpotController
{
    public interface ISpotController
    {
        /// <summary>
        /// See your wallet balances 
        /// </summary>
        [Get("/spot/getWallets")]
        Task<GetWalletsResponse> GetWalletBalancesAsync();

        /// <summary>
        /// Get all limit order 
        /// </summary>
        [Get("/spot/getLimitOrders")]
        Task<GetLimitOrdersResponse> GetLimitOrdersAsync();

        /// <summary>
        /// Get order by Id
        /// </summary>
        [Get("/spot/limitOrderStatus")]
        Task<OrderModel> LimitOrderStatusAsync(string orderId);

        /// <summary>
        /// Get order by Id
        /// </summary>
        [Get("/spot/marketOrderStatus")]
        Task<OrderModel> MarketOrderStatusAsync(string orderId);

        /// <summary>
        /// Create Limit order 
        /// </summary>
        [Post("/spot/createLimitOrder")]
        Task<OrderIdResponse> CreateLimitOrderAsync(LimitOrderRequest request);

        /// <summary>
        /// Cancel order 
        /// </summary>
        [Post("/spot/cancelOrder")]
        Task<CancelLimitOrderResponse> CancelLimitOrderAsync(CancelLimitOrderRequest request);

        /// <summary>
        /// Replace limit order. Cancel one and create new. 
        /// </summary>
        [Post("/spot/replaceLimitOrder")]
        Task<OrderIdResponse> ReplaceLimitOrderAsync(ReplaceLimitOrderRequest request);

        /// <summary>
        /// Create Market order 
        /// </summary>
        [Post("/spot/createMarketOrder")]
        Task<OrderIdResponse> CreateMarketOrderAsync(MarketOrderRequest request);

        /// <summary>
        /// View your inactive (history) orders
        /// </summary>
        [Get("/spot/getOrdersHistory")]
        Task<GetOrdersHistoryResponse> GetOrdersHistoryAsync();
    }
}