using System;

namespace Lykke.Service.AssetDisclaimers.Core.Domain
{
    public interface IDisclaimer
    {
        string Id { get; }
        string LykkeEntityId { get; }
        DisclaimerType Type { get; }
        string Name { get; }
        string Text { get; }
        DateTime StartDate { get; }
    }
}
