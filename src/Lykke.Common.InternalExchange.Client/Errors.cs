using JetBrains.Annotations;

namespace Lykke.Common.InternalExchange.Client
{
    /// <summary>
    /// Contains general errors that can be occurred during internal trading. 
    /// </summary>
    [PublicAPI]
    public static class Errors
    {
        /// <summary>
        /// Used if robot doesn't currently trade this asset pair.
        /// </summary>
        public const string AssetPairNotSupported = "Asset pair not supported.";

        /// <summary>
        /// Used if client doesn't have enough balance.
        /// </summary>
        public const string NotEnoughFunds = "Not enough funds to execute trade.";

        /// <summary>
        /// Used if robot was unable to transfer funds from client's wallet.
        /// </summary>
        public const string CannotTransferFunds = "Can't transfer funds from client's wallet.";

        /// <summary>
        /// Used if no orders were found to match original requirements.
        /// </summary>
        public const string NotEnoughLiquidity = "Not enough liquidity for requested price.";

        /// <summary>
        /// Used if requested volume was less than minimum volume for the given asset pair.
        /// </summary>
        public const string TooSmallVolume = "Requested volume too small.";

        /// <summary>
        /// Used if requested volume's accuracy was different than the given asset pair's accuracy.
        /// </summary>
        public const string InvalidVolume = "Requested volume invalid.";
    }
}