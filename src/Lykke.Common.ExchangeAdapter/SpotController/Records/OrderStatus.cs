using System.Runtime.Serialization;

namespace Lykke.Common.ExchangeAdapter.SpotController.Records
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Active")] Active,
        [EnumMember(Value = "Canceled")] Canceled,
        [EnumMember(Value = "Fill")] Fill
    }
}