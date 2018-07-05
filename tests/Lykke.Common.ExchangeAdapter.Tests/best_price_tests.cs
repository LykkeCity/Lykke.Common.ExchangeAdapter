using System;
using Lykke.Common.ExchangeAdapter.Contracts;
using NUnit.Framework;

namespace Lykke.Common.ExchangeAdapter.Tests
{
    public sealed class best_price_tests
    {
        [Test]
        public void order_book()
        {
            var ob = new OrderBook("source", "asset", DateTime.UtcNow, new[]
            {
                new OrderBookItem(100, 5),
                new OrderBookItem(101, 5),
            }, new[]
            {
                new OrderBookItem(99, 5),
                new OrderBookItem(97, 5),
            });

            Assert.AreEqual(100M, ob.BestAskPrice);
            Assert.AreEqual(99M, ob.BestBidPrice);

            var tp = TickPrice.FromOrderBook(ob);


            Assert.AreEqual(100M, tp.Ask);
            Assert.AreEqual(99M, tp.Bid);
        }

        [Test]
        public void tickprice_from_empty_orderbook()
        {
            var ts = DateTime.UtcNow;

            var ob = new OrderBook("source", "asset", ts, new OrderBookItem []{}, new OrderBookItem[]{});

            var tp = TickPrice.FromOrderBook(ob);

            Assert.AreEqual(ts, tp.Timestamp);
            Assert.AreEqual("source", tp.Source);
            Assert.AreEqual("ASSET", tp.Asset);
            Assert.AreEqual(0M, tp.Ask);
            Assert.AreEqual(0M, tp.Bid);
        }
    }
}