using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Repositories;

namespace Lykke.Service.AssetDisclaimers.AzureRepositories
{
    public class LykkeEntityRepository : ILykkeEntityRepository
    {
        private readonly INoSQLTableStorage<LykkeEntityEntity> _storage;

        public LykkeEntityRepository(INoSQLTableStorage<LykkeEntityEntity> storage)
        {
            _storage = storage;
        }
        
        public async Task<IReadOnlyList<ILykkeEntity>> GetAsync()
        {
            IEnumerable<LykkeEntityEntity> entities = await _storage.GetDataAsync(GetPartitionKey());

            return Mapper.Map<List<LykkeEntity>>(entities);
        }

        public async Task<ILykkeEntity> GetAsync(string lykkeEntityId)
        {
            LykkeEntityEntity entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey(lykkeEntityId));

            return Mapper.Map<LykkeEntity>(entity);
        }

        public async Task<ILykkeEntity> InsertAsync(ILykkeEntity disclaimer)
        {
            var entity = new LykkeEntityEntity(GetPartitionKey(), GetRowKey());
            Mapper.Map(disclaimer, entity);

            await _storage.InsertAsync(entity);

            return Mapper.Map<LykkeEntity>(entity);
        }

        public async Task UpdateAsync(ILykkeEntity disclaimer)
        {
            await _storage.MergeAsync(GetPartitionKey(), GetRowKey(disclaimer.Id), entity =>
            {
                Mapper.Map(disclaimer, entity);
                return entity;
            });
        }

        public async Task DeleteAsync(string lykkeEntityId)
        {
            await _storage.DeleteAsync(GetPartitionKey(), GetRowKey(lykkeEntityId));
        }
        
        private static string GetPartitionKey()
            => "LykkeEntity";

        private static string GetRowKey(string lykkeEntityId)
            => lykkeEntityId;

        private static string GetRowKey()
            => Guid.NewGuid().ToString("D");
    }
}
