namespace Lykke.Common.ExchangeAdapter.Tools.ObservableWebSocket
{
    public interface IMessageReceived<out T> : ISocketEvent
    {
        T Content { get; }
    }
}
