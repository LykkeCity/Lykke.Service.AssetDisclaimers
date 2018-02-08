using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Client.Models.LykkeEntities;
using Refit;

namespace Lykke.Service.AssetDisclaimers.Client.Api
{
    internal interface ILykkeEntitiesApi
    {
        [Get("/api/lykkeentities")]
        Task<IReadOnlyList<LykkeEntityModel>> GetAsync();

        [Get("/api/lykkeentities/{lykkeEntityId}")]
        Task<LykkeEntityModel> GetByIdAsync(string lykkeEntityId);

        [Post("/api/lykkeentities")]
        Task<LykkeEntityModel> AddAsync([Body] CreateLykkeEntityModel model);
        
        [Put("/api/lykkeentities")]
        Task UpdateAsync([Body] EditLykkeEntityModel model);

        [Delete("/api/lykkeentities/{lykkeEntityId}")]
        Task DeleteAsync(string lykkeEntityId);
    }
}
