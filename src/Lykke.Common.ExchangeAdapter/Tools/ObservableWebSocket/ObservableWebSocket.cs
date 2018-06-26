using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.Common.ExchangeAdapter.Tools.ObservableWebSocket
{
    public sealed class ObservableWebSocket : IObservable<ISocketEvent>
    {
        private sealed class MessageReceived : IMessageReceived<byte[]>
        {
            public MessageReceived(WebSocketSession session, byte[] content)
            {
                Content = content;
                Session = session;
            }

            public byte[] Content { get; }
            public WebSocketSession Session { get; }
        }

        private readonly string _url;
        private readonly Action<string> _log;
        private readonly IEnumerable<Type> _skipMessages;
        private readonly WebSocketTimeouts _timeouts;
        private readonly IObservable<ISocketEvent> _messages;

        public ObservableWebSocket(
            string url,
            Action<string> log,
            WebSocketTimeouts? timeouts = null,
            IEnumerable<Type> skipMessages = null)
        {
            _url = url;
            _log = log;
            _skipMessages = skipMessages;
            _timeouts = timeouts ?? WebSocketTimeouts.Default;
            _messages = Observable.Create<ISocketEvent>(async (obs, ct) => { await ReadBytesLoop(obs, log, ct); });
        }

        public IDisposable Subscribe(IObserver<ISocketEvent> observer)
        {
            return _messages.Subscribe(observer);
        }

        private async Task ReadBytesLoop(IObserver<ISocketEvent> obs, Action<string> log, CancellationToken ct)
        {
            using (var ws = new ClientWebSocket())
            {
                using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ct))
                {
                    cts.CancelAfter(_timeouts.ConnectTimeout);

                    await ws.ConnectAsync(new Uri(_url), cts.Token);
                    _log($"Connected to {_url}");
                }

                using (var session = new WebSocketSession(ws, log, _skipMessages))
                {
                    obs.OnNext(new SocketConnected(session));

                    var buffer = new byte[4096];

                    var closeSignalReceived = false;

                    while (!ct.IsCancellationRequested && !closeSignalReceived)
                    {
                        using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ct))
                        {
                            cts.CancelAfter(_timeouts.ReadTimeout);

                            using (var ms = new MemoryStream())
                            {
                                WebSocketReceiveResult result;

                                do
                                {
                                    result = await ws.ReceiveAsync(buffer, cts.Token);

                                    if (result.MessageType == WebSocketMessageType.Close)
                                    {
                                        _log("Close signal received");
                                        obs.OnCompleted();
                                        closeSignalReceived = true;
                                        break;
                                    }

                                    ms.Write(buffer, 0, result.Count);
                                } while (!result.EndOfMessage);

                                obs.OnNext(new MessageReceived(session, ms.ToArray()));
                            }
                        }
                    }
                }
            }
        }
    }
}
