using System.Runtime.Serialization;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public enum TradeType
    {
        [EnumMember(Value = "Buy")] Buy,
        [EnumMember(Value = "Sell")] Sell
    }
}