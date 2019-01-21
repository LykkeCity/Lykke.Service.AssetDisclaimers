using System;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.AssetDisclaimers.Models.ClientDisclaimers
{
    public class ClientDisclaimerModel
    {
        public string Id { get; set; }
        public string LykkeEntityId { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public DisclaimerType Type { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime ApprovedDate { get; set; }
        public bool ShowOnEachAction { get; set; }
    }
}
