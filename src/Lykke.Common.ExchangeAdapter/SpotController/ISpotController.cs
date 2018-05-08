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
        Task<GetWalletsResponse> GetWalletBalances();

        /// <summary>
        /// Get all limit order 
        /// </summary>
        [Get("/spot/getLimitOrders")]
        Task<GetLimitOrdersResponse> GetLimitOrders(string orderIds, string instruments);

        /// <summary>
        /// Get order by Id
        /// </summary>
        [Get("/spot/limitOrderStatus")]
        Task<OrderModel> LimitOrderStatus(long orderId);

        /// <summary>
        /// Get order by Id
        /// </summary>
        [Get("/spot/marketOrderStatus")]
        Task<OrderModel> MarketOrderStatus(long orderId);

        /// <summary>
        /// Create Limit order 
        /// </summary>
        [Post("/spot/createLimitOrder")]
        Task<OrderIdResponse> CreateLimitOrder(LimitOrderRequest request);

        /// <summary>
        /// Cancel order 
        /// </summary>
        [Post("/spot/cancelOrder")]
        Task<CancelLimitOrderResponse> CancelLimitOrder(CancelLimitOrderRequest request);

        /// <summary>
        /// Replace limit order. Cancel one and create new. 
        /// </summary>
        [Get("/spot/replaceLimitOrder")]
        Task<OrderIdResponse> ReplaceLimitOrder(ReplaceLimitOrderRequest request);

        /// <summary>
        /// Create Market order 
        /// </summary>
        [Post("/spot/createMarketOrder")]
        Task<OrderIdResponse> CreateMarketOrder(MarketOrderRequest request);

        /// <summary>
        /// View your inactive (history) orders
        /// </summary>
        [Get("/spot/getOrdersHistory")]
        Task<GetOrdersHistoryResponse> GetOrdersHistory();
    }
}