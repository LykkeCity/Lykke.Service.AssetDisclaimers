using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Client.Models.ClientDisclaimers;
using Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers;
using Lykke.Service.AssetDisclaimers.Client.Models.LykkeEntities;

namespace Lykke.Service.AssetDisclaimers.Client
{
    /// <summary>
    /// HTTP client for Asset disclaimers service.
    /// </summary>
    public interface IAssetDisclaimersClient
    {
        /// <summary>
        /// Returns a collection of Lykke entities.
        /// </summary>
        /// <returns>A collection of Lykke entities.</returns>
        Task<IReadOnlyList<LykkeEntityModel>> GetLykkeEntitiesAsync();

        /// <summary>
        /// Returns a Lykke entity.
        /// </summary>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <returns>The Lykke entity.</returns>
        Task<LykkeEntityModel> GetLykkeEntityAsync(string lykkeEntityId);

        /// <summary>
        /// Adds a Lykke entity.
        /// </summary>
        /// <param name="model">The Lykke entity creation information.</param>
        /// <returns>The created Lykke entity.</returns>
        Task<LykkeEntityModel> AddLykkeEntityAsync(CreateLykkeEntityModel model);

        /// <summary>
        /// Updates a Lykke entity.
        /// </summary>
        /// <param name="model">The Lykke entity update information.</param>
        Task UpdateLykkeEntityAsync(EditLykkeEntityModel model);

        /// <summary>
        /// Deletes a Lykke entity.
        /// </summary>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        Task DeleteLykkeEntityAsync(string lykkeEntityId);

        /// <summary>
        /// Returns a collection of Lykke entity disclaimers.
        /// </summary>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <returns>A collection of Lykke entity disclaimers.</returns>
        Task<IReadOnlyList<DisclaimerModel>> GetDisclaimersAsync(string lykkeEntityId);

        /// <summary>
        /// Returns a Lykke entity disclaimer.
        /// </summary>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <param name="disclaimerId">The disclaimer id.</param>
        /// <returns>A Lykke entity disclaimer.</returns>
        Task<DisclaimerModel> GetDisclaimerAsync(string lykkeEntityId, string disclaimerId);

        /// <summary>
        /// Creates a Lykke entity disclaimer.
        /// </summary>
        /// <param name="model">The Lykke entity disclaimer creation information.</param>
        /// <returns>The Lykke entity disclaimer.</returns>
        Task<DisclaimerModel> AddDisclaimerAsync(CreateDisclaimerModel model);

        /// <summary>
        /// Updates a Lykke entity disclaimer.
        /// </summary>
        /// <param name="model">The Lykke entity disclaimer update information.</param>
        Task UpdateDisclaimerAsync(EditDisclaimerModel model);

        /// <summary>
        /// Deletes a Lykke entity disclaimer.
        /// </summary>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <param name="disclaimerId">The disclaimer id.</param>
        Task DeleteDisclaimerAsync(string lykkeEntityId, string disclaimerId);

        /// <summary>
        /// Returns client approved disclaimers.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>A collection of disclaimers.</returns>
        Task<IReadOnlyList<ClientDisclaimerModel>> GetApprovedClientDisclaimersAsync(string clientId);

        /// <summary>
        /// Returns client disclaimers requires approval.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <returns>A collection of disclaimers.</returns>
        Task<IReadOnlyList<DisclaimerModel>> GetPendingClientDisclaimersAsync(string clientId);

        /// <summary>
        /// Finds an actual for today tradable disclaimer by higher priority Lykke entity and requires client approval if it has not yet been approved.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="lykkeEntityId1">The Lykke entity id.</param>
        /// <param name="lykkeEntityId2">The Lykke entity id.</param>
        /// <returns>A check result.</returns>
        Task<CheckResultModel> CheckTradableClientDisclaimerAsync(string clientId, string lykkeEntityId1, string lykkeEntityId2);

        /// <summary>
        /// Finds an actual for today deposit disclaimer by Lykke entity and requires client approval if it has not yet been approved.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <returns>A check result.</returns>
        Task<CheckResultModel> CheckDepositClientDisclaimerAsync(string clientId, string lykkeEntityId);

        /// <summary>
        /// Finds an actual for today withdrawal disclaimer by Lykke entity and requires client approval if it has not yet been approved.
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="lykkeEntityId">The Lykke entity id.</param>
        /// <returns>A check result.</returns>
        Task<CheckResultModel> CheckWithdrawalClientDisclaimerAsync(string clientId, string lykkeEntityId);

        /// <summary>
        /// Sets client disclimer as approved. 
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="disclaimerId">The disclaimer id.</param>
        Task ApproveClientDisclaimerAsync(string clientId, string disclaimerId);

        /// <summary>
        /// Sets client disclimer as declined. 
        /// </summary>
        /// <param name="clientId">The client id.</param>
        /// <param name="disclaimerId">The disclaimer id.</param>
        Task DeclineClientDisclaimerAsync(string clientId, string disclaimerId);
    }
}
