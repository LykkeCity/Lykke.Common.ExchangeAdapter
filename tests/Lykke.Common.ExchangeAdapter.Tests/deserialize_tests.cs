using System;
using System.Linq;
using Lykke.Common.ExchangeAdapter.Contracts;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Common.ExchangeAdapter.Tests
{
    public class deserialize_tests
    {
        [Test]
        public void order_book()
        {
            var ob = new OrderBook("source", "asset", DateTime.UtcNow, new[]
            {
                new OrderBookItem(100, 5),
            }, new[]
            {
                new OrderBookItem(100, 5),
            });

            var serialized = JsonConvert.SerializeObject(ob);

            Console.WriteLine(serialized);

            var deserialized = JsonConvert.DeserializeObject<OrderBook>(serialized);

            Assert.AreEqual(ob, deserialized);
        }

        [Test]
        public void serialization_sample()
        {
            var serialized =
                "{\"source\":\"source\",\"asset\":\"ASSET\",\"timestamp\":\"2018-07-05T09:27:45.661257Z\"," +
                "\"asks\":[{\"price\":100.0  ," +
                "\"volume\": \"5.0\" \n    },\n{\"price\":120.0  ," +
                "\"volume\": \"5.0\" \n    }],\"bids\":[{\"price\":100.0,\"volume\":5.0}]}";

            var deserialized = JsonConvert.DeserializeObject<OrderBook>(serialized);

            var sample = new OrderBook("source", "asset", DateTime.Parse("2018-07-05T09:27:45.661257Z"), new[]
            {
                new OrderBookItem(100, 5),
                new OrderBookItem(120, 5),
            }, new[]
            {
                new OrderBookItem(100, 5),
            });

            Assert.AreEqual(sample, deserialized);
        }

        [Test]
        public void deserialization_of_the_same_price_level_should_not_fail()
        {
            var serialized =
                "{\"source\":\"source\",\"asset\":\"ASSET\",\"timestamp\":\"2018-07-05T09:27:45.661257Z\"," +
                "\"asks\":[{\"price\":100.0  ," +
                "\"volume\": \"5.0\" \n    },\n{\"price\":100.0  ," +
                "\"volume\": \"6.0\" \n    }],\"bids\":[{\"price\":80.0,\"volume\":5.0}]}";

            var deserialized = JsonConvert.DeserializeObject<OrderBook>(serialized);

            Assert.AreEqual(11M, deserialized.AskLevels[100M]);
        }

        [Test]
        public void orderbook_items_are_sorted()
        {
            var ob = new OrderBook
            (
                "",
                "",
                DateTime.UtcNow,
                asks: new[] {4, 6, 5}.Select(x => new OrderBookItem(x, 5)),
                bids: new[] {3, 1, 2}.Select(x => new OrderBookItem(x, 5))
            );

            Assert.AreEqual(new[] {4M, 5M, 6M}, ob.AskLevels.Keys);
            Assert.AreEqual(new[] {3M, 2M, 1M}, ob.BidLevels.Keys);
        }

        [Test]
        public void best_bid_ask()
        {
            var ob1 = new OrderBook();
            Assert.AreEqual(0M, ob1.BestAskPrice);
            Assert.AreEqual(0M, ob1.BestBidPrice);

            var ob2 = new OrderBook
            (
                "",
                "",
                DateTime.UtcNow,
                asks: new[] {4, 6, 5}.Select(x => new OrderBookItem(x, 5)),
                bids: new[] {3, 1, 2}.Select(x => new OrderBookItem(x, 5))
            );

            Assert.AreEqual(4M, ob2.BestAskPrice);
            Assert.AreEqual(3M, ob2.BestBidPrice);
        }
    }
}