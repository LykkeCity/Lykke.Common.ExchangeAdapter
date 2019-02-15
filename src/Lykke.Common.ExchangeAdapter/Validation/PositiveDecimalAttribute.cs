using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Common.ExchangeAdapter.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class PositiveDecimalAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            if (decimal.TryParse(value.ToString(), out decimal number))
                return number > 0;

            return false;
        }
    }
}