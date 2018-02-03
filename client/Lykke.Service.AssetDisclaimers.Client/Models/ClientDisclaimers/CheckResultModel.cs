namespace Lykke.Service.AssetDisclaimers.Client.Models.ClientDisclaimers
{
    /// <summary>
    /// Represents a check client disclaimers result.
    /// </summary>
    public class CheckResultModel
    {
        /// <summary>
        /// If <c>true</c> client have an unaproved disclaimers, otherwise <c>false</c>.
        /// </summary>
        public bool RequiresApproval { get; set; }
    }
}
