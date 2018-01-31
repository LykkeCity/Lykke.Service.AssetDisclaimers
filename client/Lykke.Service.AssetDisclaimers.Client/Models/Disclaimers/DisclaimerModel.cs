using System;

namespace Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers
{
    public class DisclaimerModel
    {
        public string Id { get; set; }
        public string LykkeEntityId { get; set; }
        public DisclaimerType Type { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
    }
}
