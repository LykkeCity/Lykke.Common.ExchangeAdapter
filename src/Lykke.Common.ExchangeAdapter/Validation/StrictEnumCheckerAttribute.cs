using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Common.ExchangeAdapter.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class StrictEnumCheckerAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;

            if (!value.GetType().IsEnum) return false;

            return Enum.IsDefined(value.GetType(), value);
        }
    }
}