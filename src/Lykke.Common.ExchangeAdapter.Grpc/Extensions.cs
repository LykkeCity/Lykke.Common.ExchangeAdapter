using Lykke.Common.Proto;

namespace Lykke.Common.ExchangeAdapter.Grpc
{
    public static class ProtoExtensions
    {
        public static System.DateTime ToSystem(this DateTimeP dateTime)
        {
            return System.DateTime.SpecifyKind(new System.DateTime((long)dateTime.Ticks), System.DateTimeKind.Utc);
        }

        public static DateTimeP ToProto(this System.DateTime dateTime)
        {
            return new DateTimeP { Ticks = (ulong)dateTime.Ticks };
        }

        public static System.Decimal ToSystem(this DecimalP dec)
        {
            return new System.Decimal(new [] { dec.Lo, dec.Mid, dec.Hi, dec.SignScale });
        }

        public static DecimalP ToProto(this System.Decimal dec)
        {
            var bits = System.Decimal.GetBits(dec);

            return new DecimalP
            {
                Lo = bits[0],
                Mid = bits[1],
                Hi = bits[2],
                SignScale = bits[3]
            };
        }
    }
}