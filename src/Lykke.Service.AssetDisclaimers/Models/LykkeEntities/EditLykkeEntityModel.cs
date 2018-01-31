using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.AssetDisclaimers.Models.LykkeEntities
{
    public class EditLykkeEntityModel
    {
        [Required]
        public string Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public int Priority { get; set; }
    }
}
