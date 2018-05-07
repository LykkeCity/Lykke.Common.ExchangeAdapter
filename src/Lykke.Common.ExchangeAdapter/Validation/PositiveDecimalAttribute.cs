using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Common.ExchangeAdapter.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class PositiveDecimalAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }

            if (decimal.TryParse(value.ToString(), out var n))
            {
                return n > 0;
            }

            return false;
        }
    }
}