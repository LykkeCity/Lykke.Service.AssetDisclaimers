using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers;
using Refit;

namespace Lykke.Service.AssetDisclaimers.Client.Api
{
    internal interface IDisclaimersApi
    {
        [Get("/api/lykkeentities/{lykkeEntityId}/disclaimers")]
        Task<IReadOnlyList<DisclaimerModel>> GetAsync(string lykkeEntityId);
        
        [Get("/api/lykkeentities/{lykkeEntityId}/disclaimers/{disclaimerId}")]
        Task<DisclaimerModel> GetByIdAsync(string lykkeEntityId, string disclaimerId);

        [Post("/api/lykkeentities/disclaimers")]
        Task<DisclaimerModel> AddAsync([Body] CreateDisclaimerModel model);
        
        [Put("/api/lykkeentities/disclaimers")]
        Task UpdateAsync([Body] EditDisclaimerModel model);

        [Delete("/apilykkeentities/{lykkeEntityId}/disclaimers/{disclaimerId}")]
        Task DeleteAsync(string lykkeEntityId, string disclaimerId);
    }
}
