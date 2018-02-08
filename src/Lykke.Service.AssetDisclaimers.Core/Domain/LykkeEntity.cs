namespace Lykke.Service.AssetDisclaimers.Core.Domain
{
    public class LykkeEntity : ILykkeEntity
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
    }
}
