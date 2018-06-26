using System;

namespace Lykke.Common.ExchangeAdapter.Tools.ObservableWebSocket
{
    public static class MessageReceivedExtensions
    {
        private sealed class MessageReceived<T> : IMessageReceived<T>
        {
            public MessageReceived(WebSocketSession session, T content)
            {
                Content = content;
                Session = session;
            }

            public T Content { get; }
            public WebSocketSession Session { get; }
        }

        public static IMessageReceived<TResult> Convert<TSource, TResult>(
            this IMessageReceived<TSource> source,
            Func<TSource, TResult> mapFunc)
        {
            return new MessageReceived<TResult>(source.Session, mapFunc(source.Content));
        }

        public static ISocketEvent Convert<TSource, TResult>(
            this ISocketEvent source,
            Func<TSource, TResult> mapFunc)
        {
            if (source is IMessageReceived<TSource> s)
            {
                return new MessageReceived<TResult>(source.Session, mapFunc(s.Content));
            }

            return source;
        }
    }
}
