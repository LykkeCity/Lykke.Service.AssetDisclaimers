using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Exceptions;
using Lykke.Service.AssetDisclaimers.Core.Repositories;
using Lykke.Service.AssetDisclaimers.Core.Services;

namespace Lykke.Service.AssetDisclaimers.Services
{
    public class ClientDisclaimerService : IClientDisclaimerService
    {
        private readonly IClientDisclaimerRepository _clientDisclaimerRepository;
        private readonly ILykkeEntityRepository _lykkeEntityRepository;
        private readonly IDisclaimerRepository _disclaimerRepository;
        private readonly ILog _log;

        public ClientDisclaimerService(
            IClientDisclaimerRepository clientDisclaimerRepository,
            ILykkeEntityRepository lykkeEntityRepository,
            IDisclaimerRepository disclaimerRepository,
            ILog log)
        {
            _clientDisclaimerRepository = clientDisclaimerRepository;
            _lykkeEntityRepository = lykkeEntityRepository;
            _disclaimerRepository = disclaimerRepository;
            _log = log;
        }
        
        public async Task<IReadOnlyList<IClientDisclaimer>> GetApprovedAsync(string clientId)
        {
            IReadOnlyList<IClientDisclaimer> clientDisclaimers = await _clientDisclaimerRepository.GetAsync(clientId);

            return clientDisclaimers
                .Where(o => o.Approved)
                .ToList();
        }
        
        public async Task<IReadOnlyList<IClientDisclaimer>> GetPendingAsync(string clientId)
        {
            IReadOnlyList<IClientDisclaimer> clientDisclaimers = await _clientDisclaimerRepository.GetAsync(clientId);

            return clientDisclaimers
                .Where(o => !o.Approved)
                .ToList();
        }

        public async Task<bool> CheckTradableAsync(string clientId, string lykkeEntityId1, string lykkeEntityId2)
        {
            ILykkeEntity lykkeEntity1 = await _lykkeEntityRepository.GetAsync(lykkeEntityId1);
            
            if(lykkeEntity1 == null)
                throw new LykkeEntityNotFoundException(lykkeEntityId1);
            
            ILykkeEntity lykkeEntity2 = await _lykkeEntityRepository.GetAsync(lykkeEntityId2);

            if(lykkeEntity1 == null)
                throw new LykkeEntityNotFoundException(lykkeEntityId2);

            ILykkeEntity lykkeEntity = lykkeEntity1.Priority > lykkeEntity2.Priority ? lykkeEntity1 : lykkeEntity2; 
           
            return await CheckDisclaimerAsync(clientId, lykkeEntity.Id, DisclaimerType.Tradable);
        }

        public async Task<bool> CheckDepositAsync(string clientId, string lykkeEntityId)
        {
            ILykkeEntity lykkeEntity = await _lykkeEntityRepository.GetAsync(lykkeEntityId);
            
            if(lykkeEntity == null)
                throw new LykkeEntityNotFoundException(lykkeEntityId);
           
            return await CheckDisclaimerAsync(clientId, lykkeEntity.Id, DisclaimerType.Deposit);
        }
        
        public async Task<bool> CheckWithdrawalAsync(string clientId, string lykkeEntityId)
        {
            ILykkeEntity lykkeEntity = await _lykkeEntityRepository.GetAsync(lykkeEntityId);
            
            if(lykkeEntity == null)
                throw new LykkeEntityNotFoundException(lykkeEntityId);
           
            return await CheckDisclaimerAsync(clientId, lykkeEntity.Id, DisclaimerType.Withdrawal);
        }

        public async Task ApproveAsync(string clientId, string disclaimerId)
        {
            IDisclaimer disclaimer = await _disclaimerRepository.FindAsync(disclaimerId);
            
            if(disclaimer == null)
                throw new DisclaimerNotFoundException(disclaimerId);

            await _clientDisclaimerRepository.InsertOrReplaceAsync(new ClientDisclaimer
            {
                ClientId = clientId,
                DisclaimerId = disclaimerId,
                Approved = true,
                ApprovedDate = DateTime.UtcNow
            });
            
            await _log.WriteInfoAsync(nameof(ClientDisclaimerService), nameof(ApproveAsync),
                new
                {
                    clientId,
                    disclaimerId
                }.ToJson(), "Client disclaimer approved");
        }

        public async Task DeclineAsync(string clientId, string disclaimerId)
        {
            await _clientDisclaimerRepository.DeleteAsync(clientId, disclaimerId);
            
            await _log.WriteInfoAsync(nameof(ClientDisclaimerService), nameof(DeclineAsync),
                new
                {
                    clientId,
                    disclaimerId
                }.ToJson(), "Client disclaimer declined");
        }

        private async Task<bool> CheckDisclaimerAsync(string clientId, string lykkeEntityId, DisclaimerType type)
        {
            IReadOnlyList<IClientDisclaimer> clientDisclaimers = await _clientDisclaimerRepository.GetAsync(clientId);

            HashSet<string> approvedDisclaimers = clientDisclaimers
                .Where(o => o.Approved)
                .Select(o => o.DisclaimerId)
                .ToHashSet();
            
            IReadOnlyList<IDisclaimer> disclaimers = await _disclaimerRepository.GetAsync(lykkeEntityId);

            IDisclaimer requiresApprovalDisclaimer = disclaimers
                .Where(o => o.Type == type)
                .Where(o => o.StartDate < DateTime.UtcNow && !approvedDisclaimers.Contains(o.Id))
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefault();

            if (requiresApprovalDisclaimer == null)
                return false;
            
            await _clientDisclaimerRepository.InsertOrReplaceAsync(new ClientDisclaimer
            {
                ClientId = clientId,
                DisclaimerId = requiresApprovalDisclaimer.Id
            });
                
            await _log.WriteInfoAsync(nameof(ClientDisclaimerService), nameof(CheckTradableAsync),
                new
                {
                    clientId,
                    requiresApprovalDisclaimer.Id
                }.ToJson(), "Client pending disclaimer added");

            return true;
        }
    }
}
