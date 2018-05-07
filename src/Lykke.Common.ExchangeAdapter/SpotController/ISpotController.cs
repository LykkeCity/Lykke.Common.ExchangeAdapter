using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.SpotController.Records;

namespace Lykke.Common.ExchangeAdapter.SpotController
{
    public interface ISpotController
    {
        /// <summary>
        /// See your wallet balances 
        /// </summary>
        Task<GetWalletsResponse> GetWalletBalances();

        /// <summary>
        /// Get all limit order 
        /// </summary>
        Task<GetLimitOrdersResponse> GetLimitOrders(string orderIds, string instruments);

        /// <summary>
        /// Get order by Id
        /// </summary>
        Task<OrderModel> LimitOrderStatus(long orderId);

        /// <summary>
        /// Get order by Id
        /// </summary>
        Task<OrderModel> MarketOrderStatus(long orderId);

        /// <summary>
        /// Create Limit order 
        /// </summary>
        Task<OrderIdResponse> CreateLimitOrder(LimitOrderRequest request);

        /// <summary>
        /// Cancel order 
        /// </summary>
        Task<CancelLimitOrderResponse> CancelLimitOrder(CancelLimitOrderRequest request);

        /// <summary>
        /// Replace limit order. Cancel one and create new. 
        /// </summary>
        Task<OrderIdResponse> ReplaceLimitOrder(ReplaceLimitOrderRequest request);

        /// <summary>
        /// Create Market order 
        /// </summary>
        Task<OrderIdResponse> CreateMarketOrder(MarketOrderRequest request);

        /// <summary>
        /// View your inactive (history) orders
        /// </summary>
        Task<GetOrdersHistoryResponse> GetOrdersHistory();
    }
}