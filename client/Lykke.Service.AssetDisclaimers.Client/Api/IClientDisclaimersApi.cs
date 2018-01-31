using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers;
using Refit;

namespace Lykke.Service.AssetDisclaimers.Client.Api
{
    internal interface IClientDisclaimersApi
    {
        [Get("/api/clients/{clientId}/disclaimers/pending")]
        Task<IReadOnlyList<DisclaimerModel>> GetPendingAsync(string clientId);
        
        [Get("/api/clients/{clientId}/disclaimers")]
        Task<IReadOnlyList<DisclaimerModel>> GetNotApprovedAsync(string clientId, [Query] string lykkeEntityId1, [Query] string lykkeEntityId2);
        
        [Post("/api/clients/{clientId}/disclaimers/{disclaimerId}")]
        Task AddPendingAsync(string clientId, string disclaimerId);
        
        [Post("/api/clients/{clientId}/disclaimers/{disclaimerId}/approve")]
        Task ApproveAsync(string clientId, string disclaimerId);
        
        [Post("/api/clients/{clientId}/disclaimers/{disclaimerId}/decline")]
        Task DeclineAsync(string clientId, string disclaimerId);
    }
}
