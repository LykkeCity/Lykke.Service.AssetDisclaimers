using System;
using Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers;

namespace Lykke.Service.AssetDisclaimers.Client.Models.ClientDisclaimers
{
    /// <summary>
    /// Represens a client disclaimer details.
    /// </summary>
    public class ClientDisclaimerModel
    {
        /// <summary>
        /// The disclaimer id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Lykke entity id.
        /// </summary>
        public string LykkeEntityId { get; set; }

        /// <summary>
        /// The disclaimer type.
        /// </summary>
        public DisclaimerType Type { get; set; }

        /// <summary>
        /// The disclaimer name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The disclaimer text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The disclaimer start date.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The disclaimer approved date.
        /// </summary>
        public DateTime ApprovedDate { get; set; }
    }
}
