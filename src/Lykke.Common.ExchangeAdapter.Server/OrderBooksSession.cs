using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Lykke.Common.ExchangeAdapter.Contracts;

namespace Lykke.Common.ExchangeAdapter.Server
{
    public sealed class OrderBooksSession : IDisposable
    {
        public IReadOnlyCollection<string> Instruments => _byAsset.Keys.ToArray();
        public readonly IObservable<ICollection<TickPrice>> TickPrices;
        public readonly IObservable<Unit> Worker;

        private readonly ConcurrentDictionary<string, IObservable<OrderBook>> _byAsset =
            new ConcurrentDictionary<string, IObservable<OrderBook>>(StringComparer.InvariantCultureIgnoreCase);
        private readonly CompositeDisposable _disposable;

        [Obsolete("Instruments parameter is not required")]
        public OrderBooksSession(
            IEnumerable<string> instruments,
            IObservable<TickPrice> tickPrices,
            IObservable<OrderBook> orderBooks,
            IObservable<Unit> worker)
            : this(tickPrices, orderBooks, worker)
        {
        }

        public OrderBooksSession(
            IObservable<TickPrice> tickPrices,
            IObservable<OrderBook> orderBooks,
            IObservable<Unit> worker)
        {
            TickPrices = CombineTickPrices(tickPrices).ShareLatest();
            Worker = worker;

            var groupOrderBooks = orderBooks
                .GroupBy(x => x.Asset)
                .Subscribe(x => _byAsset.TryAdd(x.Key, x.ShareLatest()));

            _disposable = new CompositeDisposable(
                groupOrderBooks,
                TickPrices.Subscribe());
        }

        private static IObservable<ICollection<TickPrice>> CombineTickPrices(IObservable<TickPrice> tickPrices)
        {
            return tickPrices.Scan(new ConcurrentDictionary<string, TickPrice>(),
                    (d, tp) =>
                    {
                        d[tp.Asset] = tp;
                        return d;

                    })
                .Select(x => x.Values);
        }

        public IObservable<OrderBook> GetOrderBook(string asset)
        {
            if (asset == null) return Observable.Empty<OrderBook>();
            if (_byAsset.TryGetValue(asset, out var orderBooks)) return orderBooks;
            return Observable.Empty<OrderBook>();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}