using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Lykke.Common.ExchangeAdapter.Tests
{
    public class observable_tests
    {
        [Test]
        public async Task retry_with_publish()
        {
            var seq = Enumerable.Range(1, 5);

            var obs = seq.ToObservable().Select(x =>
            {
                if (x % 4 == 0)
                {
                    throw new Exception();
                }
                else
                {
                    return x;
                }
            });

            var shared = obs
                .Replay()
                .RefCount()
                .Retry()
                .Timeout(TimeSpan.FromMilliseconds(100));

             var result = await shared.Take(5).ToArray();

            Assert.AreEqual(new[] {1, 2, 3, 1, 2}, result);
        }

        [Test, Explicit]
        public async Task exponential_timeouts()
        {
            using (var api = Enumerable.Repeat(true, 3).Concat(Enumerable.Repeat(false, 20)).GetEnumerator())
            {
                var source = Observable.Create<DateTime>(async (obs, ct) =>
                {
                    if (api.MoveNext())
                    {
                        if (api.Current)
                        {
                            obs.OnNext(DateTime.UtcNow);

                            await Task.Delay(TimeSpan.FromMilliseconds(100), ct);

                            obs.OnNext(DateTime.UtcNow);

                            obs.OnError(new Exception("Sequence end"));
                        }
                        else
                        {
                            obs.OnError(new Exception("False received"));
                        }
                    }
                    else
                    {
                        obs.OnCompleted();
                    }
                });

                await source
                    .Do(
                        x => Console.WriteLine($"[{DateTime.UtcNow.TimeOfDay:c}]: Ok"),
                        err => Console.WriteLine($"[{DateTime.UtcNow.TimeOfDay:c}]: {err.Message}"))
                    .RetryWithBackoff(TimeSpan.FromMilliseconds(50), TimeSpan.FromSeconds(1))
                    .ToArray();
            }
        }
    }
}