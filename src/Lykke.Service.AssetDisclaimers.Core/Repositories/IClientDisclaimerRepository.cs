using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Core.Domain;

namespace Lykke.Service.AssetDisclaimers.Core.Repositories
{
    public interface IClientDisclaimerRepository
    {
        Task<IReadOnlyList<IClientDisclaimer>> GetAsync(string clientId);

        Task<IClientDisclaimer> GetAsync(string clientId, string disclaimerId);
        
        Task<IReadOnlyList<IClientDisclaimer>> FindAsync(string disclaimerId);

        Task InsertOrReplaceAsync(IClientDisclaimer clientDisclaimer);
        
        Task DeleteAsync(string clientId, string disclaimerId);
    }
}
