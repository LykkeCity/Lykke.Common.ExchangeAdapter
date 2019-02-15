using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.SpotController;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Common.ExchangeAdapter.Server
{
    [Route("api/OrderBook")]
    public abstract class OrderBookControllerBase : Controller, IOrderBookController
    {
        protected abstract OrderBooksSession Session { get; }

        [HttpGet("GetAllInstruments")]
        public IReadOnlyCollection<string> GetAllInstruments()
        {
            return Session.Instruments.ToArray();
        }

        [HttpGet("GetAllTickPrices")]
        public async Task<IReadOnlyCollection<TickPrice>> GetAllTickPrices()
        {
            return (await Session.TickPrices.FirstOrDefaultAsync())?.ToArray();
        }

        [HttpGet("GetOrderBook")]
        public async Task<OrderBook> GetOrderBook(string assetPair)
        {
            return await Session.GetOrderBook(assetPair).FirstOrDefaultAsync();
        }
    }
}