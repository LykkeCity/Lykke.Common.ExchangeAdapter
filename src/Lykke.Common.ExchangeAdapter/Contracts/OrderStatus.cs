using System.Runtime.Serialization;

namespace Lykke.Common.ExchangeAdapter.Contracts
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Active")] Active,
        [EnumMember(Value = "Canceled")] Canceled,
        [EnumMember(Value = "Fill")] Fill
    }
}