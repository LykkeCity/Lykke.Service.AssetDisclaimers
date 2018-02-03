using System;

namespace Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers
{
    /// <summary>
    /// Represents disclaimer creation information.
    /// </summary>
    public class CreateDisclaimerModel
    {
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
    }
}
