using System;

namespace Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers
{
    /// <summary>
    /// Represents disclaimer edit information.
    /// </summary>
    public class EditDisclaimerModel
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
        /// Disclaimer will require approve for each operation
        /// E.g. trade attempt -> disclaimer shown -> approved -> one trade allowed -> disclaimer will appear for next trade
        /// </summary>
        public bool ShowOnEachAction { get; set; }
    }
}
