using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Lykke.Common.ExchangeAdapter.Tools.ObservableWebSocket
{
    public sealed class WebSocketSession : IDisposable
    {
        private readonly ClientWebSocket _client;
        private readonly Action<string> _log;
        private readonly SemaphoreSlim _writeLock = new SemaphoreSlim(1);
        private readonly TimeSpan _sendTimeout = TimeSpan.FromSeconds(10);

        public WebSocketSession(
            ClientWebSocket client,
            Action<string> log,
            IEnumerable<Type> skipMessages)
        {
            _client = client;
            _log = log;

            _dontLogCommands = new HashSet<Type>(skipMessages ?? Enumerable.Empty<Type>());
        }

        private readonly HashSet<Type> _dontLogCommands;

        public async Task SendAsJson<T>(T cmd)
        {
            try
            {
                await _writeLock.WaitAsync();
                var str = JsonConvert.SerializeObject(cmd);

                if (!_dontLogCommands.Contains(typeof(T)))
                {
                    _log($"Sending: {str}");
                }

                using (var cts = new CancellationTokenSource(_sendTimeout))
                {
                    await _client.SendAsync(
                        Encoding.UTF8.GetBytes(str),
                        WebSocketMessageType.Text,
                        true,
                        cts.Token);
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            _writeLock?.Dispose();
        }
    }
}
