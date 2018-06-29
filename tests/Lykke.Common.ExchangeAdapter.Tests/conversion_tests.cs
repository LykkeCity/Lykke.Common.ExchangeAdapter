using System;
using Lykke.Common.ExchangeAdapter.Contracts;
using NUnit.Framework;

namespace Lykke.Common.ExchangeAdapter.Tests
{
    public class conversion_tests
    {
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