namespace Lykke.Service.AssetDisclaimers.Core.Domain
{
    public interface ILykkeEntity
    {
        string Id { get; }
        string Name { get; }
        string Description { get; }
        int Priority { get; }
    }
}
