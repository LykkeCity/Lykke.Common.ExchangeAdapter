using System;

namespace Lykke.Common.ExchangeAdapter.Tools.ObservableWebSocket
{
    public struct WebSocketTimeouts
    {
        public WebSocketTimeouts(
            TimeSpan connectTimeout,
            TimeSpan readTimeout,
            TimeSpan writeToSocket)
        {
            WriteToSocket = writeToSocket;
            ReadTimeout = readTimeout;
            ConnectTimeout = connectTimeout;
        }

        public readonly TimeSpan WriteToSocket;
        public readonly TimeSpan ConnectTimeout;
        public readonly TimeSpan ReadTimeout;

        public static WebSocketTimeouts Default = new WebSocketTimeouts(
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(10),
            TimeSpan.FromSeconds(30)
        );
    }
}
