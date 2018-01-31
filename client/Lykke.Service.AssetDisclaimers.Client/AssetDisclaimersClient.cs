using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Client.Api;
using Lykke.Service.AssetDisclaimers.Client.Models.ClientDisclaimers;
using Lykke.Service.AssetDisclaimers.Client.Models.Disclaimers;
using Lykke.Service.AssetDisclaimers.Client.Models.LykkeEntities;
using Microsoft.Extensions.PlatformAbstractions;
using Refit;

namespace Lykke.Service.AssetDisclaimers.Client
{
    public class AssetDisclaimersClient : IAssetDisclaimersClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly ILykkeEntitiesApi _lykkeEntitiesApi;
        private readonly IDisclaimersApi _disclaimersApi;
        private readonly IClientDisclaimersApi _clientDisclaimersApi;
        private readonly ApiRunner _runner;
        
        public AssetDisclaimersClient(AssetDisclaimersServiceClientSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrEmpty(settings.ServiceUrl))
                throw new ArgumentException("Service URL Required");
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(settings.ServiceUrl),
                DefaultRequestHeaders =
                {
                    {
                        "User-Agent",
                        $"{PlatformServices.Default.Application.ApplicationName}/{PlatformServices.Default.Application.ApplicationVersion}"
                    }
                }
            };

            _lykkeEntitiesApi = RestService.For<ILykkeEntitiesApi>(_httpClient);
            _disclaimersApi = RestService.For<IDisclaimersApi>(_httpClient);
            _clientDisclaimersApi = RestService.For<IClientDisclaimersApi>(_httpClient);
            
            _runner = new ApiRunner();
        }

        public async Task<IReadOnlyList<LykkeEntityModel>> GetLykkeEntitiesAsync()
        {
            return await _runner.RunAsync(() => _lykkeEntitiesApi.GetAsync());
        }

        public async Task<LykkeEntityModel> GetLykkeEntityAsync(string lykkeEntityId)
        {
            return await _runner.RunAsync(() => _lykkeEntitiesApi.GetByIdAsync(lykkeEntityId));
        }

        public async Task<LykkeEntityModel> AddLykkeEntityAsync(CreateLykkeEntityModel model)
        {
            return await _runner.RunAsync(() => _lykkeEntitiesApi.AddAsync(model));
        }

        public async Task UpdateLykkeEntityAsync(EditLykkeEntityModel model)
        {
            await _runner.RunAsync(() => _lykkeEntitiesApi.UpdateAsync(model));
        }

        public async Task DeleteLykkeEntityAsync(string lykkeEntityId)
        {
            await _runner.RunAsync(() => _lykkeEntitiesApi.DeleteAsync(lykkeEntityId));
        }

        public async Task<IReadOnlyList<DisclaimerModel>> GetDisclaimersAsync(string lykkeEntityId)
        {
            return await _runner.RunAsync(() => _disclaimersApi.GetAsync(lykkeEntityId));
        }

        public async Task<DisclaimerModel> GetDisclaimerAsync(string lykkeEntityId, string disclaimerId)
        {
            return await _runner.RunAsync(() => _disclaimersApi.GetByIdAsync(lykkeEntityId, disclaimerId));
        }

        public async Task<DisclaimerModel> AddDisclaimerAsync(CreateDisclaimerModel model)
        {
            return await _runner.RunAsync(() => _disclaimersApi.AddAsync(model));
        }

        public async Task UpdateDisclaimerAsync(EditDisclaimerModel model)
        {
            await _runner.RunAsync(() => _disclaimersApi.UpdateAsync(model));
        }

        public async Task DeleteDisclaimerAsync(string lykkeEntityId, string disclaimerId)
        {
            await _runner.RunAsync(() => _disclaimersApi.DeleteAsync(lykkeEntityId, disclaimerId));
        }

        public async Task<IReadOnlyList<DisclaimerModel>> GetApprovedClientDisclaimersAsync(string clientId)
        {
            return await _runner.RunAsync(() => _clientDisclaimersApi.GetApprovedAsync(clientId));
        }
        
        public async Task<IReadOnlyList<DisclaimerModel>> GetPendingClientDisclaimersAsync(string clientId)
        {
            return await _runner.RunAsync(() => _clientDisclaimersApi.GetPendingAsync(clientId));
        }

        public async Task<CheckResultModel> CheckTradableClientDisclaimerAsync(string clientId, string lykkeEntityId1, string lykkeEntityId2)
        {
            return await _runner.RunAsync(() => _clientDisclaimersApi.CheckTradableAsync(clientId, lykkeEntityId1, lykkeEntityId2));
        }

        public async Task<CheckResultModel> CheckDepositClientDisclaimerAsync(string clientId, string lykkeEntityId)
        {
            return await _runner.RunAsync(() => _clientDisclaimersApi.CheckDepositAsync(clientId, lykkeEntityId));
        }
        
        public async Task<CheckResultModel> CheckWithdrawalClientDisclaimerAsync(string clientId, string lykkeEntityId)
        {
            return await _runner.RunAsync(() => _clientDisclaimersApi.CheckWithdrawalAsync(clientId, lykkeEntityId));
        }
        
        public async Task ApproveClientDisclaimerAsync(string clientId, string disclaimerId)
        {
            await _runner.RunAsync(() => _clientDisclaimersApi.ApproveAsync(clientId, disclaimerId));
        }

        public async Task DeclineClientDisclaimerAsync(string clientId, string disclaimerId)
        {
            await _runner.RunAsync(() => _clientDisclaimersApi.DeclineAsync(clientId, disclaimerId));
        }
        
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
