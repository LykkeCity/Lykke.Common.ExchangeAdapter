using System.Linq;
using Lykke.Common.ExchangeAdapter.Contracts;
using NUnit.Framework;

namespace Lykke.Common.ExchangeAdapter.Tests
{
    public sealed class equality_tests
    {
        [Test]
        public void compare_empty_orderbooks()
        {
            var ob1 = new OrderBook {Asset = "asset"};
            var ob2 = new OrderBook {Asset = "asset"};
            Assert.IsTrue(ob1.Equals(ob2));
        }

        [Test]
        public void add_one_item_to_each_orderbook()
        {
            var ob1 = new OrderBook {Asset = "asset"};
            var ob2 = new OrderBook {Asset = "asset"};

            ob1.UpdateAsk(10, 5);
            ob2.UpdateAsk(10, 5);

            Assert.IsTrue(ob1.Equals(ob2));
        }

        [Test]
        public void add_different_items_to_each_orderbook()
        {
            var ob1 = new OrderBook {Asset = "asset"};
            var ob2 = new OrderBook {Asset = "asset"};

            ob1.UpdateAsk(10, 5);
            ob2.UpdateAsk(10, 7);

            Assert.IsFalse(ob1.Equals(ob2));
        }

        [Test]
        public void add_different_items_to_each_orderbook_with_different_keys()
        {
            var ob1 = new OrderBook {Asset = "asset"};
            var ob2 = new OrderBook {Asset = "asset"};

            ob1.UpdateAsk(10, 5);
            ob2.UpdateAsk(8, 7);

            Assert.IsFalse(ob1.Equals(ob2));
        }

        [Test]
        public void asks_sorted_ascend()
        {
            var ob = new OrderBook {Asset = "asset"};
            ob.UpdateAsk(1, 1);
            ob.UpdateAsk(3, 1);
            ob.UpdateAsk(2, 1);

            var askPrices = ob.Asks.Keys.ToArray();

            Assert.AreEqual(3, askPrices.Length);
            Assert.AreEqual(new[] {1M, 2M, 3M}, askPrices);
        }

        [Test]
        public void bids_sorted_descend()
        {
            var ob = new OrderBook {Asset = "asset"};
            ob.UpdateBid(1, 1);
            ob.UpdateBid(3, 1);
            ob.UpdateBid(2, 1);

            var bidsPrices = ob.Bids.Keys.ToArray();

            Assert.AreEqual(3, bidsPrices.Length);
            Assert.AreEqual(new[] {3M, 2M, 1M}, bidsPrices);
        }
    }
}