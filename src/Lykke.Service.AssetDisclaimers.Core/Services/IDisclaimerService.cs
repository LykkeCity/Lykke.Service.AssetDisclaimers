using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Core.Domain;

namespace Lykke.Service.AssetDisclaimers.Core.Services
{
    public interface IDisclaimerService
    {
        Task<IReadOnlyList<IDisclaimer>> GetAsync(string lykkeEntityId);
        Task<IDisclaimer> GetAsync(string lykkeEntityId, string disclaimerId);
        Task<IDisclaimer> AddAsync(IDisclaimer disclaimer);
        Task UpdateAsync(IDisclaimer disclaimer);
        Task DeleteAsync(string lykkeEntityId, string disclaimerId);
    }
}
