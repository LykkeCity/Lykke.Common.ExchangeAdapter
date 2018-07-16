using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.SpotController;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Lykke.Common.ExchangeAdapter.Server
{
    public abstract class OrderBookControllerBase : Controller, IOrderBookController
    {
        protected abstract OrderBooksSession Session { get; }

        [SwaggerOperation("GetAllInstruments")]
        [HttpGet("GetAllInstruments")]
        public IReadOnlyCollection<string> GetAllInstruments()
        {
            return Session.Instruments.ToArray();
        }

        [SwaggerOperation("GetAllTickPrices")]
        [HttpGet("GetAllTickPrices")]
        public async Task<IReadOnlyCollection<TickPrice>> GetAllTickPrices()
        {
            return (await Session.TickPrices.FirstOrDefaultAsync())?.ToArray();
        }

        [SwaggerOperation("GetOrderBook")]
        [HttpGet("GetOrderBook")]
        public async Task<OrderBook> GetOrderBook(string assetPair)
        {
            return await Session.GetOrderBook(assetPair).FirstOrDefaultAsync();
        }
    }
}