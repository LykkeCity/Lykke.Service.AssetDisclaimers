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
        Task<IReadOnlyList<DisclaimerModel>> GetApprovedAsync(string clientId);
            
        [Get("/api/clients/{clientId}/disclaimers/pending")]
        Task<IReadOnlyList<DisclaimerModel>> GetPendingAsync(string clientId);
        
        [Get("/api/clients/{clientId}/disclaimers/tradable")]
        Task<CheckResultModel> CheckTradableAsync(string clientId, [Query] string lykkeEntityId1, [Query] string lykkeEntityId2);
        
        [Get("/api/clients/{clientId}/disclaimers/deposit")]
        Task<CheckResultModel> CheckDepositAsync(string clientId, [Query] string lykkeEntityId);
        
        [Get("/api/clients/{clientId}/disclaimers/withdrawal")]
        Task<CheckResultModel> CheckWithdrawalAsync(string clientId, [Query] string lykkeEntityId);
        
        [Post("/api/clients/{clientId}/disclaimers/{disclaimerId}/approve")]
        Task ApproveAsync(string clientId, string disclaimerId);
        
        [Post("/api/clients/{clientId}/disclaimers/{disclaimerId}/decline")]
        Task DeclineAsync(string clientId, string disclaimerId);
    }
}
