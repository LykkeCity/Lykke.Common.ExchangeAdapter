using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Common.Log;
using Lykke.Common.ExchangeAdapter.Contracts;
using Lykke.Common.ExchangeAdapter.Server.Settings;
using Lykke.Common.Log;
using Lykke.RabbitMqBroker.Publisher;
using Lykke.RabbitMqBroker.Subscriber;

namespace Lykke.Common.ExchangeAdapter.Server
{
    public static class OrderBookPipelineExtensions
    {
        public static OrderBooksSession FromRawOrderBooks(
            this IObservable<OrderBook> rawOrderBooks,
            IReadOnlyCollection<string> instruments,
            OrderBookProcessingSettings settings,
            ILogFactory logFactory)
        {
            var log = logFactory.CreateLog(rawOrderBooks);

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
                    logFactory)
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
                        "OrderBooks received from WebSocket in the last {0}: {1}")
                    .NeverIfNotEnabled(publishTickPrices || publishOrderBooks),

                tpPublisher.ReportStatistics(statWindow, log, "TickPrices published in the last {0}: {1}")
                    .NeverIfNotEnabled(publishTickPrices),

                obPublisher.ReportStatistics(statWindow, log, "OrderBooks published in the last {0}: {1}")
                    .NeverIfNotEnabled(publishOrderBooks)
            );

            return new OrderBooksSession(
                instruments,
                tickPrices,
                orderBooks,
                publisher);
        }

        private struct MidPrice
        {
            public readonly decimal BestAsk;
            public readonly decimal BestBid;
            public decimal Mid => (BestAsk + BestBid) / 2;
            public string Asset;

            public MidPrice(decimal bid, decimal ask, string asset)
            {
                BestAsk = ask;
                BestBid = bid;
                Asset = asset;
            }

            public static MidPrice? Get(OrderBook ob)
            {
                if (ob.Asks.Any() && ob.Bids.Any())
                {
                    return new MidPrice(
                        ob.BestAskPrice,
                        ob.BestBidPrice,
                        ob.Asset);
                }

                return null;
            }

            public override string ToString()
            {
                return $"{Asset}: {Mid} ({BestBid} .. {BestAsk})";
            }
        }

        private static IObservable<OrderBook> DetectAndFilterAnomaliesAssumingSingleInstrument(
            this IObservable<OrderBook> source,
            ILog log)
        {
            string DetectAnomaly(MidPrice? previousMidPrice, MidPrice? midPrice)
            {
                if (previousMidPrice == null) return null;
                if (midPrice == null) return null;

                if (midPrice.Value.Mid / previousMidPrice.Value.Mid > 10M
                    || previousMidPrice.Value.Mid / midPrice.Value.Mid > 10M)
                {
                    return $"Found anomaly, orderbook skipped. " +
                           $"Current midPrice is " +
                           $"{previousMidPrice.Value}, the new one is {midPrice.Value}";
                }
                else
                {
                    return null;
                }
            }

            return Observable.Create<OrderBook>(async (obs, ct) =>
            {
                MidPrice? prevMid = null;

                await source.ForEachAsync(orderBook =>
                {
                    var newMidPrice = MidPrice.Get(orderBook);
                    var anomaly = DetectAnomaly(prevMid, newMidPrice);

                    if (anomaly != null)
                    {
                        log.Warning(anomaly);
                    }
                    else
                    {
                        prevMid = newMidPrice ?? prevMid;
                        obs.OnNext(orderBook);
                    }
                }, ct);
            });
        }

        public static IObservable<OrderBook> DetectAndFilterAnomalies(
            this IObservable<OrderBook> source,
            ILog log,
            IEnumerable<string> skipAssets)
        {
            var assetsToSkip = new HashSet<string>(skipAssets.Select(x => x.ToUpperInvariant()));

            return source
                .GroupBy(x => x.Asset)
                .SelectMany(group => assetsToSkip.Contains(group.Key.ToUpperInvariant())
                    ? group
                    : group.DetectAndFilterAnomaliesAssumingSingleInstrument(log));
        }

        public static IObservable<T> NeverIfNotEnabled<T>(this IObservable<T> source, bool enabled)
        {
            return enabled ? source : Observable.Never<T>();
        }

        public static IObservable<OrderBook> OnlyWithPositiveSpread(this IObservable<OrderBook> source)
        {
            return source.Where(x => !x.TryDetectNegativeSpread(out _));
        }

        public static IObservable<T> ThrottleEachInstrument<T>(
            this IObservable<T> source,
            Func<T, string> getAsset,
            float maxEventsPerSecond)
        {
            if (maxEventsPerSecond < 0) throw new ArgumentOutOfRangeException(nameof(maxEventsPerSecond));
            if (Math.Abs(maxEventsPerSecond) < 0.01) return source;

            return source
                .GroupBy(getAsset)
                .Select(grouped => grouped.Sample(TimeSpan.FromSeconds(1) / maxEventsPerSecond))
                .Merge();
        }

        public static IObservable<T> DistinctEveryInstrument<T>(this IObservable<T> source, Func<T, string> getAsset)
        {
            return source.GroupBy(getAsset).Select(x => x.DistinctUntilChanged()).Merge();
        }

        public static IObservable<Unit> PublishToRmq<T>(
            this IObservable<T> source,
            string connectionString,
            string exchanger,
            ILogFactory logFactory)
        {
            return PublishToRmq(source, connectionString, exchanger, logFactory, true);
        }

        public static IObservable<Unit> PublishToRmq<T>(
            this IObservable<T> source,
            string connectionString,
            string exchanger,
            ILogFactory logFactory,
            bool isDurable)
        {
            const string prefix = "lykke.";

            if (exchanger.StartsWith(prefix)) exchanger = exchanger.Substring(prefix.Length);

            var settings = RabbitMqSubscriptionSettings.CreateForPublisher(
                connectionString,
                exchanger);

            settings.IsDurable = isDurable;

            var connection
                = new RabbitMqPublisher<T>(logFactory, settings)
                    .SetSerializer(new JsonMessageSerializer<T>())
                    .SetPublishStrategy(new DefaultFanoutPublishStrategy(settings))
                    .PublishSynchronously()
                    .Start();

            return source.SelectMany(async x =>
            {
                await connection.ProduceAsync(x);
                return Unit.Default;
            });
        }

        public static IObservable<T> ReportErrors<T>(this IObservable<T> source, string process, ILog log)
        {
            return source.Do(_ => { }, err => log.Warning(process, err));
        }

        public static IObservable<Unit> ReportStatistics<T>(
            this IObservable<T> source,
            TimeSpan window,
            ILog log,
            string format = "Entities registered in the last {0}: {1}")
        {
            return source
                .WindowCount(window)
                .Sample(window)
                .Do(x => log.Info(nameof(ReportStatistics), string.Format(format, window, x)))
                .Select(_ => Unit.Default);
        }
    }
}