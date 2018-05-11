namespace Lykke.Common.Proto
{
    public partial class Decimal
    {
        public static implicit operator System.Decimal(Decimal dec)
        {
            return new System.Decimal(new [] { dec.Lo, dec.Mid, dec.Hi, dec.SignScale });
        }

        public static implicit operator Decimal(System.Decimal dec)
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