using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.AssetDisclaimers.Models.LykkeEntities
{
    public class CreateLykkeEntityModel
    {
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public int Priority { get; set; }
    }
}
