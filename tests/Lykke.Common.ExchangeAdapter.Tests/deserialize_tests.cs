using System;
using System.Diagnostics;
using System.Linq;
using Lykke.Common.ExchangeAdapter.Contracts;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Lykke.Common.ExchangeAdapter.Tests
{
    public class deserialize_tests
    {
        private static OrderBook ob;

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

            var sample = new OrderBook("source", "ASSET", DateTime.Parse("2018-07-05T09:27:45.661257Z"), new[]
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

        [Test, Repeat(10)]
        public void timing()
        {
            ob = GenerateSampleOrderBook();

            var serialized = JsonConvert.SerializeObject(ob);

            var sw = Stopwatch.StartNew();

            var count = 1000;
            for (int i = 0; i < count; i++)
            {
                var _ = JsonConvert.DeserializeObject<OrderBook>(serialized);
            }

            Console.WriteLine(sw.Elapsed);
            sw.Stop();
        }

        private OrderBook GenerateSampleOrderBook()
        {
            var middle = 8000M;
            var count = 101;
            var orderbookSpread = 0.01M;
            var depth = 0.2M;
            var volume = 0.1356M;

            var minAsk = middle * (1 + orderbookSpread / 2);
            var maxBid = middle * (1 - orderbookSpread / 2);

            var minBid = middle * (1 - depth / 2);
            var maxAsk = middle * (1 + depth / 2);

            var bidStep = (maxBid - minBid) / (count - 1);
            var askStep = (maxAsk - minAsk) / (count - 1);

            return new OrderBook("source", "ASSET", DateTime.UtcNow,
                asks: Enumerable.Range(0, count).Select(x => minAsk + x * askStep)
                    .Select(p => new OrderBookItem(p, volume)),
                bids: Enumerable.Range(0, count).Select(x => maxBid - x * bidStep)
                    .Select(p => new OrderBookItem(p, volume)));
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