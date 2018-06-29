using System;
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

            var deseriaized = JsonConvert.DeserializeObject<OrderBook>(serialized);

            Assert.AreEqual(ob, deseriaized);
        }
    }
}