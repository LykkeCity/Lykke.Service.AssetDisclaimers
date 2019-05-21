using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Exceptions;
using Lykke.Service.AssetDisclaimers.Core.Repositories;
using Lykke.Service.AssetDisclaimers.Core.Services;

namespace Lykke.Service.AssetDisclaimers.Services
{
    public class LykkeEntityService : ILykkeEntityService
    {
        private readonly RedisService _redisService;
        private readonly ILykkeEntityRepository _lykkeEntityRepository;
        private readonly IDisclaimerRepository _disclaimerRepository;
        private readonly ILog _log;

        public LykkeEntityService(
            RedisService redisService,
            ILykkeEntityRepository lykkeEntityRepository,
            IDisclaimerRepository disclaimerRepository,
            ILogFactory logFactory)
        {
            _redisService = redisService;
            _lykkeEntityRepository = lykkeEntityRepository;
            _disclaimerRepository = disclaimerRepository;
            _log = logFactory.CreateLog(this);
        }
        
        public Task<IReadOnlyList<ILykkeEntity>> GetAsync()
        {
            return _redisService.GetLykkeEntitiesAsync();
        }

        public Task<ILykkeEntity> GetAsync(string lykkeEntityId)
        {
            return _redisService.GetLykkeEntityAsync(lykkeEntityId);
        }

        public async Task<ILykkeEntity> AddAsync(ILykkeEntity lykkeEntity)
        {
            ILykkeEntity existingLykkeEntity = await _redisService.GetLykkeEntityAsync(lykkeEntity.Id);

            if (existingLykkeEntity != null)
                throw new LykkeEntityAlreadyExistsException(lykkeEntity.Id);
            
            ILykkeEntity createdLykkeEntity = await _lykkeEntityRepository.InsertAsync(lykkeEntity);
            await _redisService.AddLykkeEntityAsync(createdLykkeEntity);
            
            _log.Info("Lykke entity added", createdLykkeEntity);

            return createdLykkeEntity;
        }

        public async Task UpdateAsync(ILykkeEntity lykkeEntity)
        {
            ILykkeEntity existingLykkeEntity = await _redisService.GetLykkeEntityAsync(lykkeEntity.Id);
            
            if(existingLykkeEntity == null)
                throw new LykkeEntityNotFoundException(lykkeEntity.Id);
            
            var tasks = new List<Task>
            {
                _lykkeEntityRepository.UpdateAsync(lykkeEntity),
                _redisService.AddLykkeEntityAsync(lykkeEntity)
            };

            await Task.WhenAll(tasks);
            
            _log.Info("Lykke entity updated", lykkeEntity);
        }

        public async Task DeleteAsync(string lykkeEntityId)
        {
            IReadOnlyList<IDisclaimer> disclaimers = (await _redisService.GetDisclaimersAsync())
                .Where(x => x.LykkeEntityId == lykkeEntityId)
                .ToList();
            
            if(disclaimers.Count > 0)
                throw new InvalidOperationException("Can not delete Lykke entity if one or more disclaimers exists.");

            var tasks = new List<Task>
            {
                _lykkeEntityRepository.DeleteAsync(lykkeEntityId),
                _redisService.DeleteLykkeEntityAsync(lykkeEntityId)
            };

            await Task.WhenAll(tasks);
            
            _log.Info("Lykke entity declined", new { lykkeEntityId });
        }
    }
}
