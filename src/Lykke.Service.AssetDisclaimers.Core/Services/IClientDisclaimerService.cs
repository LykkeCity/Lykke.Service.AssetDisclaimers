using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Core.Domain;

namespace Lykke.Service.AssetDisclaimers.Core.Services
{
    public interface IClientDisclaimerService
    {
        Task<IReadOnlyList<IDisclaimer>> GetApprovedAsync(string clientId);
        
        Task<IReadOnlyList<IDisclaimer>> GetPendingAsync(string clientId);
        
        Task<bool> CheckTradableAsync(string clientId, string lykkeEntityId1, string lykkeEntityId2);
        
        Task<bool> CheckDepositAsync(string clientId, string lykkeEntityId);
        
        Task<bool> CheckWithdrawalAsync(string clientId, string lykkeEntityId);
        
        Task ApproveAsync(string clientId, string disclaimerId);

        Task DeclineAsync(string clientId, string disclaimerId);
    }
}
