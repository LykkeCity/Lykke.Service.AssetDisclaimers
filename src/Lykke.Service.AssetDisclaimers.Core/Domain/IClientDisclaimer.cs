using System;

namespace Lykke.Service.AssetDisclaimers.Core.Domain
{
    public interface IClientDisclaimer
    {
        string ClientId { get; }
        string DisclaimerId { get; }
        bool Approved { get; }
        DateTime? ApprovedDate { get; }
    }
}
