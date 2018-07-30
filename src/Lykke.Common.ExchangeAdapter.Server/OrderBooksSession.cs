using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.Server.Settings;
using Lykke.Common.Log;

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
                .Subscribe(x =>
                {
                    var subscription = x.ShareLatest();

                    if (_byAsset.TryAdd(x.Key, subscription))
                    {
                        _disposable.Add(subscription.Subscribe());
                    }
                });

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

        public static OrderBooksSession FromRawOrderBooks(
            IObservable<OrderBook> rawOrderBooks,
            OrderBookProcessingSettings settings,
            ILogFactory logFactory)
        {
            var log = logFactory.CreateLog(new OrderBookPipelines());

            var statWindow = TimeSpan.FromMinutes(1);

            var orderBooks = rawOrderBooks
                .OnlyWithPositiveSpread()
                .DetectAndFilterAnomalies(log, settings.AllowedAnomalisticAssets ?? new string[0])
                .ReportErrors(nameof(FromRawOrderBooks), log)
                .RetryWithBackoff(TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5))
                .Share();

            var tickPrices = orderBooks
                .Select(TickPrice.FromOrderBook)
                .DistinctEveryInstrument(x => x.Asset)
                .ReportErrors(nameof(FromRawOrderBooks), log)
                .RetryWithBackoff(TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5))
                .Share();

            var obPublisher =
                orderBooks
                    .ThrottleEachInstrument(x => x.Asset, settings.MaxEventPerSecondByInstrument)
                    .Select(x => x.Truncate(settings.OrderBookDepth))
                    .PublishToRmq(
                        settings.OrderBooks.ConnectionString,
                        settings.OrderBooks.Exchanger,
                        logFactory,
                        settings.OrderBooks.Durable)
                    .ReportErrors(nameof(FromRawOrderBooks), log)
                    .RetryWithBackoff(TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5))
                    .Share();

            var tpPublisher = tickPrices
                .ThrottleEachInstrument(x => x.Asset, settings.MaxEventPerSecondByInstrument)
                .PublishToRmq(
                    settings.TickPrices.ConnectionString,
                    settings.TickPrices.Exchanger,
                    logFactory,
                    settings.TickPrices.Durable)
                .ReportErrors(nameof(FromRawOrderBooks), log)
                .RetryWithBackoff(TimeSpan.FromSeconds(5), TimeSpan.FromMinutes(5))
                .Share();

            var publishTickPrices = settings.TickPrices.Enabled;
            var publishOrderBooks = settings.OrderBooks.Enabled;

            var publisher = Observable.Merge(
                tpPublisher.NeverIfNotEnabled(publishTickPrices),
                obPublisher.NeverIfNotEnabled(publishOrderBooks),

                orderBooks.ReportStatistics(
                        statWindow,
                        log,
                        "OrderBooks received from source in the last {0} - {1}")
                    .NeverIfNotEnabled(publishTickPrices || publishOrderBooks),

                tpPublisher
                    .ReportStatistics(statWindow, log, "TickPrices published in the last {0} - {1}")
                    .NeverIfNotEnabled(publishTickPrices),

                obPublisher.ReportStatistics(statWindow, log, "OrderBooks published in the last {0} - {1}")
                    .NeverIfNotEnabled(publishOrderBooks)
            );

            return new OrderBooksSession(
                tickPrices,
                orderBooks,
                publisher);
        }
    }
}
