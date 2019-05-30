using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Repositories;
using Lykke.Service.AssetDisclaimers.Services.Extensions;
using MoreLinq;
using StackExchange.Redis;

namespace Lykke.Service.AssetDisclaimers.Services
{
    public class RedisService
    {
        private readonly ILykkeEntityRepository _lykkeEntityRepository;
        private readonly IDisclaimerRepository _disclaimerRepository;
        private readonly IClientDisclaimerRepository _clientDisclaimerRepository;
        private readonly IDatabase _database;

        public RedisService(
            ILykkeEntityRepository lykkeEntityRepository,
            IDisclaimerRepository disclaimerRepository,
            IClientDisclaimerRepository clientDisclaimerRepository,
            IDatabase database
        )
        {
            _lykkeEntityRepository = lykkeEntityRepository;
            _disclaimerRepository = disclaimerRepository;
            _clientDisclaimerRepository = clientDisclaimerRepository;
            _database = database;
        }

        private static string GetDisclaimerKey(string disclaimerId) => $"AssetDisclaimers:Disclaimers:Id:{disclaimerId}";
        private static string GetDisclaimerIdsKey() => "AssetDisclaimers:DisclaimerIds";
        private static string GetLykkeEntityKey(string entityId) => $"AssetDisclaimers:LykkeEntity:Id:{entityId}";
        private static string GetLykkeEntityIdsKey() => "AssetDisclaimers:LykkeEntityIds";
        
        private static string GetClientDisclaimerKey(string clientId, string disclaimerId) => $"AssetDisclaimers:Client:{clientId}:Disclaimers:Id:{disclaimerId}";
        private static string GetClientDisclaimerIdsKey(string clientId) => $"AssetDisclaimers:Client:{clientId}:DisclaimerIds";
        
        public async Task<IDisclaimer> GetDisclaimerAsync(string id)
        {
            var data = await _database.HashGetAllAsync(GetDisclaimerKey(id));

            if ( data.Length != 0)
                return data.ToDisclaimer();

            var disclaimer = await _disclaimerRepository.FindAsync(id);

            if (disclaimer == null) 
                return null;
                
            await AddDisclaimerAsync(disclaimer);
            return disclaimer;
        }
        
        public async Task<IReadOnlyList<IDisclaimer>> GetDisclaimersAsync()
        {
            var result = new List<IDisclaimer>();
            List<string> disclaimerIds = await GetSetAsync(GetDisclaimerIdsKey());

            if (!disclaimerIds.Any())
            {
                IReadOnlyList<IDisclaimer> disclaimers = await _disclaimerRepository.GetAllAsync();

                var tasks = new List<Task>();
                
                foreach (var items in disclaimers.Batch(100))
                {
                    tasks.AddRange(items.Select(AddDisclaimerAsync));
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }

                return disclaimers;
            }

            foreach (string id in disclaimerIds)
            {
                var disclaimer = await GetDisclaimerAsync(id);

                if (disclaimer != null)
                    result.Add(disclaimer);
            }

            return result;
        }
        
        public Task AddDisclaimerAsync(IDisclaimer disclaimer)
        {
            var tasks = new List<Task>
            {
                _database.HashSetAsync(GetDisclaimerKey(disclaimer.Id), disclaimer.ToHashEntries()),
                _database.SetAddAsync(GetDisclaimerIdsKey(), disclaimer.Id)
            };

            return Task.WhenAll(tasks);
        }
        
        public Task DeleteDisclaimerAsync(string id)
        {
            var tasks = new List<Task>
            {
                _database.KeyDeleteAsync(GetDisclaimerKey(id)),
                _database.SetRemoveAsync(GetDisclaimerIdsKey(), id)
            };

            return Task.WhenAll(tasks);
        }
        
        public async Task<ILykkeEntity> GetLykkeEntityAsync(string id)
        {
            var data = await _database.HashGetAllAsync(GetLykkeEntityKey(id));

            if (data.Length != 0)
                return data.ToLykkeEntity();

            var entity = await _lykkeEntityRepository.GetAsync(id);

            if (entity == null) 
                return null;
                
            await AddLykkeEntityAsync(entity);
            return entity;
        }
        
        public async Task<IReadOnlyList<ILykkeEntity>> GetLykkeEntitiesAsync()
        {
            var result = new List<ILykkeEntity>();
            List<string> entityIds = await GetSetAsync(GetLykkeEntityIdsKey());
            
            if (!entityIds.Any())
            {
                IReadOnlyList<ILykkeEntity> entities = await _lykkeEntityRepository.GetAsync();

                var tasks = new List<Task>();
                
                foreach (var items in entities.Batch(100))
                {
                    tasks.AddRange(items.Select(AddLykkeEntityAsync));
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }

                return entities;
            }

            foreach (string id in entityIds)
            {
                var entity = await GetLykkeEntityAsync(id);

                if (entity != null)
                    result.Add(entity);
            }

            return result;
        }
        
        public Task AddLykkeEntityAsync(ILykkeEntity entity)
        {
            var tasks = new List<Task>
            {
                _database.HashSetAsync(GetLykkeEntityKey(entity.Id), entity.ToHashEntries()),
                _database.SetAddAsync(GetLykkeEntityIdsKey(), entity.Id)
            };

            return Task.WhenAll(tasks);
        }
        
        public Task DeleteLykkeEntityAsync(string id)
        {
            var tasks = new List<Task>
            {
                _database.KeyDeleteAsync(GetLykkeEntityKey(id)),
                _database.SetRemoveAsync(GetLykkeEntityIdsKey(), id)
            };

            return Task.WhenAll(tasks);
        }
        
        public async Task<IClientDisclaimer> GetClientDisclaimerAsync(string clientId, string disclaimerId)
        {
            var data = await _database.HashGetAllAsync(GetClientDisclaimerKey(clientId, disclaimerId));

            if ( data.Length != 0)
                return data.ToClientDisclaimer();

            var disclaimer = await _clientDisclaimerRepository.GetAsync(clientId, disclaimerId);

            if (disclaimer == null) 
                return null;
                
            await AddClientDisclaimerAsync(disclaimer);
            return disclaimer;
        }
        
        public async Task<IReadOnlyList<IClientDisclaimer>> GetClientDisclaimersAsync(string clientId)
        {
            var result = new List<IClientDisclaimer>();
            List<string> disclaimerIds = await GetSetAsync(GetClientDisclaimerIdsKey(clientId));
            
            if (!disclaimerIds.Any())
            {
                IReadOnlyList<IClientDisclaimer> disclaimers = await _clientDisclaimerRepository.GetAsync(clientId);

                var tasks = new List<Task>();
                
                foreach (var items in disclaimers.Batch(100))
                {
                    tasks.AddRange(items.Select(AddClientDisclaimerAsync));
                    await Task.WhenAll(tasks);
                    tasks.Clear();
                }

                return disclaimers;
            }
            
            foreach (string id in disclaimerIds)
            {
                var disclaimer = await GetClientDisclaimerAsync(clientId, id);

                if (disclaimer != null)
                    result.Add(disclaimer);
            }

            return result;
        }
        
        public Task AddClientDisclaimerAsync(IClientDisclaimer disclaimer)
        {
            var tasks = new List<Task>
            {
                _database.HashSetAsync(GetClientDisclaimerKey(disclaimer.ClientId, disclaimer.DisclaimerId), disclaimer.ToHashEntries()),
                _database.SetAddAsync(GetClientDisclaimerIdsKey(disclaimer.ClientId), disclaimer.DisclaimerId)
            };

            return Task.WhenAll(tasks);
        }
        
        public Task DeleteClientDisclaimerAsync(string clientId, string id)
        {
            var tasks = new List<Task>
            {
                _database.KeyDeleteAsync(GetClientDisclaimerKey(clientId, id)),
                _database.SetRemoveAsync(GetClientDisclaimerIdsKey(clientId), id)
            };

            return Task.WhenAll(tasks);
        }

        private async Task<List<string>> GetSetAsync(string key)
        {
            var values = await _database.SetMembersAsync(key);
            return values.Select(x => (string) x).ToList();
        }
    }
}
