using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Client.Models.ClientDisclaimers;
using Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers;
using Refit;

namespace Lykke.Service.AssetDisclaimers.Client.Api
{
    internal interface IClientDisclaimersApi
    {
        [Get("/api/clients/{clientId}/disclaimers/approved")]
        Task<IReadOnlyList<ClientDisclaimerModel>> GetApprovedAsync(string clientId);
            
        [Get("/api/clients/{clientId}/disclaimers/pending")]
        Task<IReadOnlyList<DisclaimerModel>> GetPendingAsync(string clientId);
        
        [Post("/api/clients/{clientId}/disclaimers/tradable")]
        Task<CheckResultModel> CheckTradableAsync(string clientId, string lykkeEntityId1, string lykkeEntityId2);
        
        [Post("/api/clients/{clientId}/disclaimers/deposit")]
        Task<CheckResultModel> CheckDepositAsync(string clientId, string lykkeEntityId);
        
        [Post("/api/clients/{clientId}/disclaimers/withdrawal")]
        Task<CheckResultModel> CheckWithdrawalAsync(string clientId, string lykkeEntityId);
        
        [Post("/api/clients/{clientId}/disclaimers/{disclaimerId}/approve")]
        Task ApproveAsync(string clientId, string disclaimerId);
        
        [Post("/api/clients/{clientId}/disclaimers/{disclaimerId}/decline")]
        Task DeclineAsync(string clientId, string disclaimerId);
    }
}
