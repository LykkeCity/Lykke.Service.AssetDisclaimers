using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Repositories;
using MoreLinq;

namespace Lykke.Service.AssetDisclaimers.Services
{
    public class InitService : IStartable
    {
        private readonly IDisclaimerRepository _disclaimerRepository;
        private readonly ILykkeEntityRepository _lykkeEntityRepository;
        private readonly RedisService _redisService;
        private readonly ILog _log;

        public InitService(
            IDisclaimerRepository disclaimerRepository,
            ILykkeEntityRepository lykkeEntityRepository,
            RedisService redisService,
            ILogFactory logFactory
            )
        {
            _disclaimerRepository = disclaimerRepository;
            _lykkeEntityRepository = lykkeEntityRepository;
            _redisService = redisService;
            _log = logFactory.CreateLog(this);
        }
        
        public void Start()
        {
            _log.Info("Getting disclaimers and lykke entities to cache");
            
            Task<IReadOnlyList<IDisclaimer>> disclaimersTask = _disclaimerRepository.GetAllAsync();
            Task<IReadOnlyList<ILykkeEntity>> entitiesTask = _lykkeEntityRepository.GetAsync();

            Task.WhenAll(disclaimersTask, entitiesTask).GetAwaiter().GetResult();
            
            _log.Info("Caching disclaimers");

            var tasks = new List<Task>();

            foreach (IEnumerable<IDisclaimer> disclaimers in disclaimersTask.Result.Batch(100))
            {
                tasks.AddRange(disclaimers.Select(x => _redisService.AddDisclaimerAsync(x)));
                Task.WhenAll(tasks).GetAwaiter().GetResult();
                tasks.Clear();
            }
            
            _log.Info("Caching disclaimers finished");
            
            _log.Info("Caching lykke entities");
            
            foreach (IEnumerable<ILykkeEntity> entities in entitiesTask.Result.Batch(100))
            {
                tasks.AddRange(entities.Select(x => _redisService.AddLykkeEntityAsync(x)));
                Task.WhenAll(tasks).GetAwaiter().GetResult();
                tasks.Clear();
            }
            
            _log.Info("Caching lykke entities finished");
        }
    }
}
