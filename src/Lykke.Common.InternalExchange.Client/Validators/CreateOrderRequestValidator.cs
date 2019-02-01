using FluentValidation;
using JetBrains.Annotations;
using Lykke.Common.InternalExchange.Client.Models;

namespace Lykke.Common.InternalExchange.Client.Validators
{
    /// <summary>
    /// The validator for the <see cref="CreateOrderRequest" /> model.
    /// </summary>
    [UsedImplicitly]
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="CreateOrderRequestValidator"/>.
        /// </summary>
        public CreateOrderRequestValidator()
        {
            RuleFor(o => o.WalletId)
                .NotEmpty()
                .WithMessage("Wallet id required");

            RuleFor(o => o.AssetPair)
                .NotEmpty()
                .WithMessage("Asset pair required");

            RuleFor(o => o.Type)
                .NotEqual(OrderType.None)
                .WithMessage("Type should be specified");

            RuleFor(o => o.Price)
                .GreaterThan(0)
                .WithMessage("Price should be great than zero");

            RuleFor(o => o.Volume)
                .GreaterThan(0)
                .WithMessage("Volume be great than zero");
        }
    }
}