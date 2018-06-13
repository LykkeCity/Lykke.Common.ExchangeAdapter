using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Lykke.Common.ExchangeAdapter.Tests
{
    public class observable_tests
    {
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