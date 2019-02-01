namespace Lykke.Common.InternalExchange.Client
{
    public static class Errors
    {
        /// <summary>
        /// Used if robot doesn't currently trade this asset pair.
        /// </summary>
        public const string AssetPairNotSupported = "Asset pair not supported.";

        /// <summary>
        /// Used if client doesn't have enough balance.
        /// </summary>
        public const string NotEnoughBalance = "Not enough funds to execute trade.";

        /// <summary>
        /// Used if robot was unable to transfer funds from client's wallet.
        /// </summary>
        public const string CannotTransferFunds = "Can't transfer funds from client's wallet.";

        /// <summary>
        /// Used if no orders were found to match original requirements.
        /// </summary>
        public const string NotEnoughLiquidity = "Not enough liquidity for requested price.";
    }
}