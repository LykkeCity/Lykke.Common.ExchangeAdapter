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

    }
}