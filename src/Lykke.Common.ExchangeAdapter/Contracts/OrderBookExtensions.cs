using System.Linq;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public static class OrderBookExtensions
    {
        public static bool TryDetectNegativeSpread(this OrderBook orderBook, out string error)
        {
            if (orderBook.AskLevels.Any() && orderBook.BidLevels.Any())
            {
                var bestAsk = orderBook.BestAskPrice;
                var bestBid = orderBook.BestBidPrice;
                if (bestAsk < bestBid)
                {
                    error = $"OrderBook for asset {orderBook.Asset} has negative spread, " +
                            $"minAsk: {bestAsk}, " +
                            $"maxBid: {bestBid}," +
                            $"spread: {bestAsk - bestBid}";
                    return true;
                }
            }

            error = null;
            return false;
        }
    }
}
