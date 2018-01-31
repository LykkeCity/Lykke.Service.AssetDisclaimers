using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Core.Domain;

namespace Lykke.Service.AssetDisclaimers.Core.Services
{
    public interface IClientDisclaimerService
    {
        Task<IReadOnlyList<IDisclaimer>> GetPendingAsync(string clientId);
        
        Task<IReadOnlyList<IDisclaimer>> GetNotApprovedAsync(string clientId, IReadOnlyList<string> lykkeEntities);
        
        Task AddPendingAsync(string clientId, string disclaimerId);

        Task ApproveAsync(string clientId, string disclaimerId);

        Task DeclineAsync(string clientId, string disclaimerId);
    }
}
