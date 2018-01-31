using System;
using System.ComponentModel.DataAnnotations;
using Lykke.Service.AssetDisclaimers.Core.Domain;

namespace Lykke.Service.AssetDisclaimers.Models.Disclaimers
{
    public class CreateDisclaimerModel
    {
        [Required]
        public string LykkeEntityId { get; set; }
        
        [Required]
        public DisclaimerType Type { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Text { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
    }
}
