using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Core.Domain;

namespace Lykke.Service.AssetDisclaimers.Core.Services
{
    public interface ILykkeEntityService
    {
        Task<IReadOnlyList<ILykkeEntity>> GetAsync();
        Task<ILykkeEntity> GetAsync(string lykkeEntityId);
        Task<ILykkeEntity> AddAsync(ILykkeEntity lykkeEntity);
        Task UpdateAsync(ILykkeEntity lykkeEntity);
        Task DeleteAsync(string lykkeEntityId);
    }
}
