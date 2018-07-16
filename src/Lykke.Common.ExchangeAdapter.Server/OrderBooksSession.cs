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
        public readonly IReadOnlyCollection<string> Instruments;
        public readonly IObservable<ICollection<TickPrice>> TickPrices;
        public readonly IObservable<Unit> Worker;

        private readonly Dictionary<string, IObservable<OrderBook>> _byAsset;
        private readonly CompositeDisposable _disposable;

        public OrderBooksSession(
            IReadOnlyCollection<string> instruments,
            IObservable<TickPrice> tickPrices,
            IObservable<OrderBook> orderBooks,
            IObservable<Unit> worker)
        {
            Instruments = instruments;
            TickPrices = CombineTickPrices(tickPrices).ShareLatest();
            Worker = worker;

            _byAsset = new Dictionary<string, IObservable<OrderBook>>(
                StringComparer.InvariantCultureIgnoreCase);

            foreach (var i in instruments)
            {
                var shareLatest = orderBooks.Where(x =>
                        string.Equals(x.Asset, i, StringComparison.InvariantCultureIgnoreCase))
                    .StartWith((OrderBook) null)
                    .ShareLatest();

                _byAsset[i] = shareLatest;
            }

            _disposable = new CompositeDisposable(
                _byAsset.Values
                    .Select(x => x.Subscribe())
                    .Concat(new[] {TickPrices.Subscribe()}));
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
            if (_byAsset.TryGetValue(asset, out var orderBooks)) return orderBooks;
            return Observable.Empty<OrderBook>();
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}