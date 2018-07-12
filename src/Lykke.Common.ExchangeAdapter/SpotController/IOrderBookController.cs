using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.ExchangeAdapter.Contracts;
using Refit;

namespace Lykke.Common.ExchangeAdapter.SpotController
{
    public interface IOrderBookController
    {
        [Get("/api/OrderBooks/GetAllInstruments")]
        IReadOnlyCollection<string> GetAllInstruments();

        [Get("/api/OrderBooks/GetAllTickPrices")]
        Task<IReadOnlyCollection<TickPrice>> GetAllTickPrices();

        [Get("/api/OrderBooks/GetOrderBook")]
        Task<OrderBook> GetOrderBook(string assetPair);
    }
}