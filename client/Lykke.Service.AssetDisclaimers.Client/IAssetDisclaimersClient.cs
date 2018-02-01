using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Client.Models.ClientDisclaimers;
using Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers;
using Lykke.Service.AssetDisclaimers.Client.Models.LykkeEntities;

namespace Lykke.Service.AssetDisclaimers.Client
{
    public interface IAssetDisclaimersClient
    {
        Task<IReadOnlyList<LykkeEntityModel>> GetLykkeEntitiesAsync();
        
        Task<LykkeEntityModel> GetLykkeEntityAsync(string lykkeEntityId);
        
        Task<LykkeEntityModel> AddLykkeEntityAsync(CreateLykkeEntityModel model);
        
        Task UpdateLykkeEntityAsync(EditLykkeEntityModel model);
        
        Task DeleteLykkeEntityAsync(string lykkeEntityId);
        
        Task<IReadOnlyList<DisclaimerModel>> GetDisclaimersAsync(string lykkeEntityId);
        
        Task<DisclaimerModel> GetDisclaimerAsync(string lykkeEntityId, string disclaimerId);
        
        Task<DisclaimerModel> AddDisclaimerAsync(CreateDisclaimerModel model);
        
        Task UpdateDisclaimerAsync(EditDisclaimerModel model);
        
        Task DeleteDisclaimerAsync(string lykkeEntityId, string disclaimerId);

        Task<IReadOnlyList<ClientDisclaimerModel>> GetApprovedClientDisclaimersAsync(string clientId);
        
        Task<IReadOnlyList<DisclaimerModel>> GetPendingClientDisclaimersAsync(string clientId);
        
        Task<CheckResultModel> CheckTradableClientDisclaimerAsync(string clientId, string lykkeEntityId1, string lykkeEntityId2);
        
        Task<CheckResultModel> CheckDepositClientDisclaimerAsync(string clientId, string lykkeEntityId);
        
        Task<CheckResultModel> CheckWithdrawalClientDisclaimerAsync(string clientId, string lykkeEntityId);
        
        Task ApproveClientDisclaimerAsync(string clientId, string disclaimerId);
        
        Task DeclineClientDisclaimerAsync(string clientId, string disclaimerId);
    }
}
