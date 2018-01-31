using System;

namespace Lykke.Service.AssetDisclaimers.Core.Domain
{
    public class ClientDisclaimer : IClientDisclaimer
    {
        public string ClientId { get; set; }
        public string DisclaimerId { get; set; }
        public bool Approved { get; set; }
        public DateTime? ApprovedDate { get; set; }
    }
}
