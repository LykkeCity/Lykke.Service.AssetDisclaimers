using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Core.Domain;

namespace Lykke.Service.AssetDisclaimers.Core.Repositories
{
    public interface ILykkeEntityRepository
    {
        Task<IReadOnlyList<ILykkeEntity>> GetAsync();
        Task<ILykkeEntity> GetAsync(string lykkeEntityId);
        Task<ILykkeEntity> InsertAsync(ILykkeEntity disclaimer);
        Task UpdateAsync(ILykkeEntity disclaimer);
        Task DeleteAsync(string lykkeEntityId);
    }
}
