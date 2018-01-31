using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Exceptions;
using Lykke.Service.AssetDisclaimers.Core.Repositories;
using Lykke.Service.AssetDisclaimers.Core.Services;

namespace Lykke.Service.AssetDisclaimers.Services
{
    public class LykkeEntityService : ILykkeEntityService
    {
        private readonly ILykkeEntityRepository _lykkeEntityRepository;
        private readonly IDisclaimerRepository _disclaimerRepository;
        private readonly ILog _log;

        public LykkeEntityService(
            ILykkeEntityRepository lykkeEntityRepository,
            IDisclaimerRepository disclaimerRepository,
            ILog log)
        {
            _lykkeEntityRepository = lykkeEntityRepository;
            _disclaimerRepository = disclaimerRepository;
            _log = log;
        }
        
        public async Task<IReadOnlyList<ILykkeEntity>> GetAsync()
        {
            return await _lykkeEntityRepository.GetAsync();
        }

        public async Task<ILykkeEntity> GetAsync(string lykkeEntityId)
        {
            return await _lykkeEntityRepository.GetAsync(lykkeEntityId);
        }

        public async Task<ILykkeEntity> AddAsync(ILykkeEntity lykkeEntity)
        {
            ILykkeEntity existingLykkeEntity = await _lykkeEntityRepository.GetAsync(lykkeEntity.Id);

            if (existingLykkeEntity != null)
                throw new LykkeEntityAlreadyExistsException(lykkeEntity.Id);
            
            ILykkeEntity createdLykkeEntity = await _lykkeEntityRepository.InsertAsync(lykkeEntity);
            
            await _log.WriteInfoAsync(nameof(LykkeEntityService), nameof(UpdateAsync),
                createdLykkeEntity.ToJson(), "Lykke entity added");

            return createdLykkeEntity;
        }

        public async Task UpdateAsync(ILykkeEntity lykkeEntity)
        {
            ILykkeEntity existingLykkeEntity = await _lykkeEntityRepository.GetAsync(lykkeEntity.Id);
            
            if(existingLykkeEntity == null)
                throw new LykkeEntityNotFoundException(lykkeEntity.Id);
            
            await _lykkeEntityRepository.UpdateAsync(lykkeEntity);
            
            await _log.WriteInfoAsync(nameof(LykkeEntityService), nameof(UpdateAsync),
                lykkeEntity.ToJson(), "Lykke entity updated");
        }

        public async Task DeleteAsync(string lykkeEntityId)
        {
            IReadOnlyList<IDisclaimer> disclaimers = await _disclaimerRepository.GetAsync(lykkeEntityId);
            
            if(disclaimers.Count > 0)
                throw new InvalidOperationException("Can not delete Lykke entity if one or more disclaimers exists.");

            await _lykkeEntityRepository.DeleteAsync(lykkeEntityId);
            
            await _log.WriteInfoAsync(nameof(LykkeEntityService), nameof(DeleteAsync),
                new
                {
                    lykkeEntityId
                }.ToJson(), "Lykke entity declined");
        }
    }
}
