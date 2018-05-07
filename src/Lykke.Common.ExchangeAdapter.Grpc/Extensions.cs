namespace Lykke.Common.ExchangeAdapter.Grpc
{
    public static class ProtoExtensions
    {
        public static System.DateTime ToSystem(this DateTime dateTime)
        {
            return System.DateTime.SpecifyKind(new System.DateTime((long)dateTime.Ticks), System.DateTimeKind.Utc);
        }

        public static DateTime ToProto(this System.DateTime dateTime)
        {
            return new DateTime { Ticks = (ulong)dateTime.Ticks };
        }

        public static System.Decimal ToSystem(this Decimal dec)
        {
            return new System.Decimal(new [] { dec.Lo, dec.Mid, dec.Hi, dec.SignScale });
        }

        public static Decimal ToProto(this System.Decimal dec)
        {
            var bits = System.Decimal.GetBits(dec);

            return new Decimal
            {
                Lo = bits[0],
                Mid = bits[1],
                Hi = bits[2],
                SignScale = bits[3]
            };
        }
    }
}