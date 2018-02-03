namespace Lykke.Service.AssetDisclaimers.Client.Models.LykkeEntities
{
    /// <summary>
    /// Represents a Lykke entity.
    /// </summary>
    public class LykkeEntityModel
    {
        /// <summary>
        /// The Lykke entity id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Lykke entity name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Lykke entity description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The Lykke entity priority.
        /// </summary>
        public int Priority { get; set; }
    }
}
