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
        
        public async Task<IReadOnlyList<IDisclaimer>> GetPendingAsync(string clientId)
        {
            IReadOnlyList<IClientDisclaimer> clientDisclaimers = await _clientDisclaimerRepository.GetAsync(clientId);

            List<string> clienPendingDisclaimers = clientDisclaimers
                .Where(o => !o.Approved)
                .Select(o => o.DisclaimerId)
                .ToList();

            var disclaimers = new List<IDisclaimer>();
            
            foreach (string disclaimerId in clienPendingDisclaimers)
            {
                disclaimers.Add(await _disclaimerRepository.FindAsync(disclaimerId));
            }
            
            return disclaimers;
        }

        public async Task<IReadOnlyList<IDisclaimer>> GetNotApprovedAsync(string clientId, IReadOnlyList<string> lykkeEntities)
        {
            IReadOnlyList<ILykkeEntity> entities = await _lykkeEntityRepository.GetAsync();

            ILykkeEntity lykkeEntity = entities
                .Where(o => lykkeEntities.Contains(o.Id))
                .OrderByDescending(o => o.Priority)
                .FirstOrDefault();
            
            if(lykkeEntity == null)
                return new List<IDisclaimer>();

            IReadOnlyList<IClientDisclaimer> clientDisclaimers = await _clientDisclaimerRepository.GetAsync(clientId);

            HashSet<string> approvedDisclaimers = clientDisclaimers
                .Where(o => o.Approved)
                .Select(o => o.DisclaimerId)
                .ToHashSet();

            IReadOnlyList<IDisclaimer> disclaimers = await _disclaimerRepository.GetAsync(lykkeEntity.Id);

            return disclaimers
                .Where(o => o.StartDate < DateTime.UtcNow && !approvedDisclaimers.Contains(o.Id))
                .GroupBy(o => o.Type)
                .Select(o => o.OrderByDescending(p => p.StartDate).FirstOrDefault())
                .ToList();
        }

        public async Task AddPendingAsync(string clientId, string disclaimerId)
        {
            IDisclaimer disclaimer = await _disclaimerRepository.FindAsync(disclaimerId);
            
            if(disclaimer == null)
                throw new DisclaimerNotFoundException(disclaimerId);

            await _clientDisclaimerRepository.InsertOrReplaceAsync(new ClientDisclaimer
            {
                ClientId = clientId,
                DisclaimerId = disclaimerId
            });
            
            await _log.WriteInfoAsync(nameof(ClientDisclaimerService), nameof(AddPendingAsync),
                new
                {
                    clientId,
                    disclaimerId
                }.ToJson(), "Client pending disclaimer added");
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
    }
}
