namespace Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers
{
    /// <summary>
    /// Contains disclaimer types.
    /// </summary>
    public enum DisclaimerType
    {
        /// <summary>
        /// Undefined.
        /// </summary>
        None,

        /// <summary>
        /// The tradable disclaimer.
        /// </summary>
        Tradable,

        /// <summary>
        /// The deposit disclaimer.
        /// </summary>
        Deposit,

        /// <summary>
        /// The withdrawal disclaimer.
        /// </summary>
        Withdrawal
    }
}
