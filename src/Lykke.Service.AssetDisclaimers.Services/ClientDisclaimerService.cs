using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Payments.FxPaygate.Client;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Exceptions;
using Lykke.Service.AssetDisclaimers.Core.Repositories;
using Lykke.Service.AssetDisclaimers.Core.Services;

namespace Lykke.Service.AssetDisclaimers.Services
{
    public class ClientDisclaimerService : IClientDisclaimerService
    {
        private readonly IClientDisclaimerRepository _clientDisclaimerRepository;
        private readonly TimeSpan _pendingTimeout;
        private readonly string _depositDelayDisclaimerId;
        private readonly IFxPaygateClient _fxPaygateClient;
        private readonly RedisService _redisService;
        private readonly ILog _log;

        public ClientDisclaimerService(
            IClientDisclaimerRepository clientDisclaimerRepository,
            IFxPaygateClient fxPaygateClient,
            RedisService redisService,
            TimeSpan pendingTimeout,
            string depositDelayDisclaimerId,
            ILogFactory logFactory)
        {
            _clientDisclaimerRepository = clientDisclaimerRepository;
            _pendingTimeout = pendingTimeout;
            _depositDelayDisclaimerId = depositDelayDisclaimerId;
            _fxPaygateClient = fxPaygateClient;
            _redisService = redisService;
            _log = logFactory.CreateLog(this);
        }
        
        public async Task<IReadOnlyList<IClientDisclaimer>> GetApprovedAsync(string clientId)
        {
            IReadOnlyList<IClientDisclaimer> clientDisclaimers = await _redisService.GetClientDisclaimersAsync(clientId);

            return clientDisclaimers
                .Where(o => o.Approved)
                .ToList();
        }
        
        public async Task<IReadOnlyList<IClientDisclaimer>> GetPendingAsync(string clientId)
        {
            IReadOnlyList<IClientDisclaimer> clientDisclaimers = await _redisService.GetClientDisclaimersAsync(clientId);

            return clientDisclaimers
                .Where(o => !o.Approved && o.ApprovedDate.Add(_pendingTimeout) > DateTime.UtcNow)
                .ToList();
        }

        public async Task<bool> CheckTradableAsync(string clientId, string lykkeEntityId1, string lykkeEntityId2)
        {
            ILykkeEntity lykkeEntity1 = await _redisService.GetLykkeEntityAsync(lykkeEntityId1);
            
            if(lykkeEntity1 == null)
                throw new LykkeEntityNotFoundException(lykkeEntityId1);
            
            ILykkeEntity lykkeEntity2 = await _redisService.GetLykkeEntityAsync(lykkeEntityId2);

            if(lykkeEntity2 == null)
                throw new LykkeEntityNotFoundException(lykkeEntityId2);

            ILykkeEntity lykkeEntity = lykkeEntity1.Priority > lykkeEntity2.Priority ? lykkeEntity1 : lykkeEntity2; 
            
            return await CheckDisclaimerAsync(clientId, lykkeEntity.Id, DisclaimerType.Tradable);
        }

        public async Task<bool> CheckDepositAsync(string clientId, string lykkeEntityId)
        {
            ILykkeEntity lykkeEntity = await _redisService.GetLykkeEntityAsync(lykkeEntityId); 
            
            if(lykkeEntity == null)
                throw new LykkeEntityNotFoundException(lykkeEntityId);

            if (!string.IsNullOrEmpty(_depositDelayDisclaimerId) && 
                !await _fxPaygateClient.IsClientSuspicious(clientId))
            {
                await ApproveAsync(clientId, _depositDelayDisclaimerId);
            }

            return await CheckDisclaimerAsync(clientId, lykkeEntity.Id, DisclaimerType.Deposit);
        }
        
        public async Task<bool> CheckWithdrawalAsync(string clientId, string lykkeEntityId)
        {
            ILykkeEntity lykkeEntity = await _redisService.GetLykkeEntityAsync(lykkeEntityId);
            
            if(lykkeEntity == null)
                throw new LykkeEntityNotFoundException(lykkeEntityId);
           
            return await CheckDisclaimerAsync(clientId, lykkeEntity.Id, DisclaimerType.Withdrawal);
        }

        public async Task ApproveAsync(string clientId, string disclaimerId)
        {
            IDisclaimer disclaimer = await _redisService.GetDisclaimerAsync(disclaimerId);
            
            if(disclaimer == null)
                throw new DisclaimerNotFoundException(disclaimerId);

            var clientDisclaimer = new ClientDisclaimer
            {
                ClientId = clientId,
                DisclaimerId = disclaimerId,
                Approved = true,
                ApprovedDate = DateTime.UtcNow
            };

            var tasks = new List<Task>
            {
                _clientDisclaimerRepository.InsertOrReplaceAsync(clientDisclaimer),
                _redisService.AddClientDisclaimerAsync(clientDisclaimer)
            };

            await Task.WhenAll(tasks);
            
            _log.Info("Client disclaimer approved", new { clientId, disclaimerId });
        }

        public async Task DeclineAsync(string clientId, string disclaimerId)
        {
            var tasks = new List<Task>
            {
                _clientDisclaimerRepository.DeleteAsync(clientId, disclaimerId),
                _redisService.DeleteClientDisclaimerAsync(clientId, disclaimerId)
            };

            await Task.WhenAll(tasks);
            
            _log.Info("Client disclaimer declined", new { clientId, disclaimerId });
        }

        private async Task<bool> CheckDisclaimerAsync(string clientId, string lykkeEntityId, DisclaimerType type)
        {
            IDisclaimer requiresApprovalDisclaimer = (await _redisService.GetDisclaimersAsync())
                .Where(x => x.LykkeEntityId == lykkeEntityId)
                .Where(o => o.Type == type)
                .Where(o => o.StartDate < DateTime.UtcNow)
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefault();

            if (requiresApprovalDisclaimer == null)
                return false;
            
            IReadOnlyList<IClientDisclaimer> clientDisclaimers = await _redisService.GetClientDisclaimersAsync(clientId);

            HashSet<string> approvedDisclaimers = clientDisclaimers
                .Where(o => o.Approved)
                .Select(o => o.DisclaimerId)
                .ToHashSet();

            List<Task> tasks;
            
            if (approvedDisclaimers.Contains(requiresApprovalDisclaimer.Id))
            {
                if (requiresApprovalDisclaimer.ShowOnEachAction)
                {
                    tasks = new List<Task>
                    {
                        _clientDisclaimerRepository.DeleteAsync(clientId, requiresApprovalDisclaimer.Id),
                        _redisService.DeleteClientDisclaimerAsync(clientId, requiresApprovalDisclaimer.Id)
                    };

                    await Task.WhenAll(tasks);
                }

                return false;
            }

            if (clientDisclaimers.Any(x => x.DisclaimerId == requiresApprovalDisclaimer.Id && !x.Approved))
            {
                _log.Info("Client pending disclaimer already added", new  { clientId, requiresApprovalDisclaimer.Id });
                return true;
            }

            var clientDisclaimer = new ClientDisclaimer
            {
                ClientId = clientId,
                DisclaimerId = requiresApprovalDisclaimer.Id,
                ApprovedDate = DateTime.UtcNow
            };
            
            tasks = new List<Task>
            {
                _clientDisclaimerRepository.InsertOrReplaceAsync(clientDisclaimer),
                _redisService.AddClientDisclaimerAsync(clientDisclaimer)
            };

            await Task.WhenAll(tasks);
            
            _log.Info("Client pending disclaimer added", new  { clientId, requiresApprovalDisclaimer.Id });

            return true;
        }
    }
}
