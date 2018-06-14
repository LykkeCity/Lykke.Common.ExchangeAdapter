using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Common.ExchangeAdapter
{
    public enum LockKind
    {
        Epoch,
        EpochMilliseconds
    }

    public sealed class EpochNonce : IDisposable
    {
        private static readonly ConcurrentDictionary<string, EpochNonce> Pool =
            new ConcurrentDictionary<string, EpochNonce>(StringComparer.InvariantCultureIgnoreCase);

        public static Task<T> Lock<T>(
            string userId,
            Func<long, Task<T>> code)
        {
            return Lock(userId, code, LockKind.Epoch);
        }

        public static Task<T> Lock<T>(
            string userId,
            Func<long, Task<T>> code,
            LockKind lockKind)
        {
            var nonce = Pool.GetOrAdd(userId, _ => new EpochNonce());
            return nonce.Lock(code, lockKind);
        }

        private readonly SemaphoreSlim _nonceLock = new SemaphoreSlim(1);

        private long _current;

        private async Task<T> Lock<T>(
            Func<long, Task<T>> code,
            LockKind lockKind)
        {
            await _nonceLock.WaitAsync();

            try
            {
                long epoch;

                switch (lockKind)
                {
                    case LockKind.Epoch:
                        epoch = DateTime.UtcNow.Epoch();
                        break;
                    case LockKind.EpochMilliseconds:
                        epoch = DateTime.UtcNow.EpochMilliseconds();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(lockKind), lockKind, null);
                }


                if (epoch > _current)
                {
                    _current = epoch;
                }
                else
                {
                    _current++;
                }

                return await code(_current);
            }
            finally
            {
                _nonceLock.Release();
            }
        }

        public void Dispose()
        {
            _nonceLock?.Dispose();
        }
    }
}