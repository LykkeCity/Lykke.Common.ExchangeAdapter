using System.Linq;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public static class OrderBookExtensions
    {
        public static bool TryDetectNegativeSpread(this OrderBook orderBook, out string info)
        {
            if (orderBook.Asks.Any() && orderBook.Bids.Any())
            {
                var bestAsk = orderBook.Asks.Min(ob => ob.Price);
                var bestBid = orderBook.Bids.Max(ob => ob.Price);
                if (bestAsk < bestBid)
                {
                    info = $"Orderbook for asset {orderBook.Asset} has negative spread, " +
                           $"minAsk: {bestAsk}, " +
                           $"maxBid: {bestBid}," +
                           $"spread: {bestAsk - bestBid}";
                    return true;
                }
            }

            info = null;
            return false;
        }
    }
}