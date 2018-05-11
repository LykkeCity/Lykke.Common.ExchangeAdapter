using Lykke.Common.Proto;

namespace Lykke.Common.Proto
{
    public partial class DateTime
    {
        public static implicit operator System.DateTime(DateTime dateTime)
        {
            return System.DateTime.SpecifyKind(new System.DateTime((long)dateTime.Ticks), System.DateTimeKind.Utc);
        }

        public static implicit operator DateTime(System.DateTime dateTime)
        {
            return new DateTime { Ticks = (ulong)dateTime.Ticks };
        }
    }
}