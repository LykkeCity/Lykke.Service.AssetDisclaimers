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
    public class DisclaimerService : IDisclaimerService
    {
        private readonly RedisService _redisService;
        private readonly IDisclaimerRepository _disclaimerRepository;
        private readonly IClientDisclaimerRepository _clientDisclaimerRepository;
        private readonly ILog _log;

        public DisclaimerService(
            RedisService redisService,
            IDisclaimerRepository disclaimerRepository,
            IClientDisclaimerRepository clientDisclaimerRepository,
            ILogFactory logFactory)
        {
            _redisService = redisService;
            _disclaimerRepository = disclaimerRepository;
            _clientDisclaimerRepository = clientDisclaimerRepository;
            _log = logFactory.CreateLog(this);
        }
        
        public async Task<IReadOnlyList<IDisclaimer>> GetAsync(string lykkeEntityId)
        {
            return (await _redisService.GetDisclaimersAsync())
                .Where(x => x.LykkeEntityId == lykkeEntityId).ToList();
        }

        public async Task<IDisclaimer> GetAsync(string lykkeEntityId, string disclaimerId)
        {
            return (await _redisService.GetDisclaimersAsync())
                .FirstOrDefault(x => x.LykkeEntityId == lykkeEntityId && x.Id == disclaimerId);
        }

        public async Task<IDisclaimer> FindAsync(string disclaimerId)
        {
            return await _redisService.GetDisclaimerAsync(disclaimerId);
        }

        public async Task<IDisclaimer> AddAsync(IDisclaimer disclaimer)
        {
            ILykkeEntity lykkeEntity = await _redisService.GetLykkeEntityAsync(disclaimer.LykkeEntityId);
            
            if(lykkeEntity == null)
                throw new LykkeEntityNotFoundException(disclaimer.LykkeEntityId);

            IDisclaimer createdDisclaimer = await _disclaimerRepository.InsertAsync(disclaimer);
            await _redisService.AddDisclaimerAsync(createdDisclaimer);
            
            _log.Info("Lykke entity disclaimer added", disclaimer);

            return createdDisclaimer;
        }

        public async Task UpdateAsync(IDisclaimer disclaimer)
        {
            IDisclaimer existingDisclaimer = await _redisService.GetDisclaimerAsync(disclaimer.Id);
            
            if(existingDisclaimer == null)
                throw new DisclaimerNotFoundException(disclaimer.Id);

            var tasks = new List<Task>
            {
                _disclaimerRepository.UpdateAsync(disclaimer), 
                _redisService.AddDisclaimerAsync(disclaimer)
            };

            await Task.WhenAll(tasks);
           
            _log.Info("Lykke entity disclaimer updated", disclaimer);
        }

        public async Task DeleteAsync(string lykkeEntityId, string disclaimerId)
        {
            IReadOnlyList<IClientDisclaimer> clientDisclaimers =
                await _clientDisclaimerRepository.FindAsync(disclaimerId);

            if (clientDisclaimers.Any(o=>o.Approved))
                throw new InvalidOperationException(
                    "Can not delete Lykke entity disclaimer if it already approved by client.");

            var tasks = new List<Task> 
            { 
                _disclaimerRepository.DeleteAsync(lykkeEntityId, disclaimerId), 
                _redisService.DeleteDisclaimerAsync(disclaimerId)
            };

            await Task.WhenAll(tasks);

            _log.Info("Lykke entity disclaimer deleted", new { lykkeEntityId, disclaimerId });
        }
    }
}
