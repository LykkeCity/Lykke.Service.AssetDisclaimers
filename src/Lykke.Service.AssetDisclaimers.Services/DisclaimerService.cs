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
    public class DisclaimerService : IDisclaimerService
    {
        private readonly IDisclaimerRepository _disclaimerRepository;
        private readonly ILykkeEntityRepository _lykkeEntityRepository;
        private readonly IClientDisclaimerRepository _clientDisclaimerRepository;
        private readonly ILog _log;

        public DisclaimerService(
            IDisclaimerRepository disclaimerRepository,
            ILykkeEntityRepository lykkeEntityRepository,
            IClientDisclaimerRepository clientDisclaimerRepository,
            ILog log)
        {
            _disclaimerRepository = disclaimerRepository;
            _lykkeEntityRepository = lykkeEntityRepository;
            _clientDisclaimerRepository = clientDisclaimerRepository;
            _log = log;
        }
        
        public async Task<IReadOnlyList<IDisclaimer>> GetAsync(string lykkeEntityId)
        {
            return await _disclaimerRepository.GetAsync(lykkeEntityId);
        }

        public async Task<IDisclaimer> GetAsync(string lykkeEntityId, string disclaimerId)
        {
            return await _disclaimerRepository.GetAsync(lykkeEntityId, disclaimerId);
        }

        public async Task<IDisclaimer> AddAsync(IDisclaimer disclaimer)
        {
            ILykkeEntity lykkeEntity = await _lykkeEntityRepository.GetAsync(disclaimer.LykkeEntityId);
            
            if(lykkeEntity == null)
                throw new LykkeEntityNotFoundException(disclaimer.LykkeEntityId);

            IDisclaimer createdDisclaimer = await _disclaimerRepository.InsertAsync(disclaimer);
            
            await _log.WriteInfoAsync(nameof(DisclaimerService), nameof(UpdateAsync),
                disclaimer.ToJson(), "Lykke entity disclaimer added");

            return createdDisclaimer;
        }

        public async Task UpdateAsync(IDisclaimer disclaimer)
        {
            IDisclaimer existingDisclaimer =
                await _disclaimerRepository.GetAsync(disclaimer.LykkeEntityId, disclaimer.Id);
            
            if(existingDisclaimer == null)
                throw new DisclaimerNotFoundException(disclaimer.Id);

            await _disclaimerRepository.UpdateAsync(disclaimer);
            
            await _log.WriteInfoAsync(nameof(DisclaimerService), nameof(UpdateAsync),
                disclaimer.ToJson(), "Lykke entity disclaimer updated");
        }

        public async Task DeleteAsync(string lykkeEntityId, string disclaimerId)
        {
            IReadOnlyList<IClientDisclaimer> clientDisclaimers =
                await _clientDisclaimerRepository.FindAsync(disclaimerId);

            if (clientDisclaimers.Any(o=>o.Approved))
                throw new InvalidOperationException(
                    "Can not delete Lykke entity disclaimer if it already approved by client.");

            await _disclaimerRepository.DeleteAsync(lykkeEntityId, disclaimerId);

            await _log.WriteInfoAsync(nameof(DisclaimerService), nameof(DeleteAsync),
                new
                {
                    lykkeEntityId,
                    disclaimerId
                }.ToJson(), "Lykke entity disclaimer deleted");
        }
    }
}
