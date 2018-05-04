using System;
using System.Collections.Generic;
using System.Reactive.Linq;

namespace Lykke.Common.ExchangeAdapter
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Samples the observable sequence at each interval.
        /// Upon each sampling tick, the latest element (if any) in the source sequence during the last sampling interval is sent to the resulting sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">Source sequence to sample.</param>
        /// <param name="interval">Interval at which to sample. If this value is equal to TimeSpan.Zero, the scheduler returns original stream.</param>
        /// <returns>Sampled observable sequence.</returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="source" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="interval" /> is less than TimeSpan.Zero.</exception>
        /// <remarks>
        /// This implementation fixes problem related to TimeSpan.Zero interval in original Observable.Sample method
        /// </remarks>
        public static IObservable<TSource> LimitFrequency<TSource>(this IObservable<TSource> source, TimeSpan? interval)
        {
            return interval.HasValue && interval.Value > TimeSpan.Zero
                ? source.Sample(interval.Value)
                : source;
        }

        public static IObservable<T> Share<T>(this IObservable<T> source)
        {
            return source.Publish().RefCount();
        }

        public static IObservable<T> ShareLatest<T>(this IObservable<T> source)
        {
            return source.Replay(1).RefCount();
        }

        private static IEnumerable<IObservable<T>> Generator<T>(IObservable<T> source, TimeSpan min, TimeSpan max)
        {
            TimeSpan? delay = null;

            while (true)
            {
                var delayed =  delay == null? source : source.DelaySubscription(delay.Value);

                var result = delayed.Do(_ => { delay = null; }, err =>
                {
                    if (delay == null) delay = min;
                    else
                    {
                        var nextMs = delay.Value.Milliseconds * 2;
                        delay = TimeSpan.FromMilliseconds(Math.Min(nextMs, max.TotalMilliseconds));
                    }
                });

                yield return result
                    .Select(x => (true, x))
                    .Catch((Exception ex) => Observable.Return((false, default(T))))
                    .Where(x => x.Item1)
                    .Select(x => x.Item2);
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public static IObservable<T> RetryWithBackoff<T>(
            this IObservable<T> source,
            TimeSpan min,
            TimeSpan max)
        {
            return Generator(source, min, max).Concat();
        }

        public static IObservable<long> WindowCount<T>(this IObservable<T> source, TimeSpan window)
        {
            var runningTotal = source.Scan((long) 0, (c, _) => c + 1).StartWith(0);

            var totalDelayed = runningTotal.Delay(window).StartWith(0);

            return Observable.CombineLatest(runningTotal, totalDelayed, (t, d) => t - d);
        }

    }
}
