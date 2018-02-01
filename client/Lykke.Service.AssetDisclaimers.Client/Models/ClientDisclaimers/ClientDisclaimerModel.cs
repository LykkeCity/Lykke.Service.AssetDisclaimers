using System;
using Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers;

namespace Lykke.Service.AssetDisclaimers.Client.Models.ClientDisclaimers
{
    public class ClientDisclaimerModel
    {
        public string Id { get; set; }
        public string LykkeEntityId { get; set; }
        public DisclaimerType Type { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ApprovedDate { get; set; }
    }
}
