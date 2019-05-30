using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Common;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Repositories;

namespace Lykke.Service.AssetDisclaimers.AzureRepositories
{
    public class DisclaimerRepository : IDisclaimerRepository
    {
        private readonly INoSQLTableStorage<DisclaimerEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _disclaimerIdIndexStorage;

        public DisclaimerRepository(
            INoSQLTableStorage<DisclaimerEntity> storage,
            INoSQLTableStorage<AzureIndex> disclaimerIdIndexStorage)
        {
            _storage = storage;
            _disclaimerIdIndexStorage = disclaimerIdIndexStorage;
        }
        
        public async Task<IReadOnlyList<IDisclaimer>> GetAllAsync()
        {
            var result = new List<Disclaimer>();

            await _storage.GetDataByChunksAsync(chunks =>
            {
                result.AddRange(Mapper.Map<List<Disclaimer>>(chunks
                    .Where(x => x.RowKey.IsGuid())));
            });

            return result;
        }

        public async Task<IDisclaimer> FindAsync(string disclaimerId)
        {
            AzureIndex index = await _disclaimerIdIndexStorage
                .GetDataAsync(GetDisclaimerIdIndexPartitionKey(disclaimerId), GetDisclaimerIdIndexRowKey());

            if (index == null)
                return null;

            DisclaimerEntity entity = await _storage.GetDataAsync(index);
            
            return Mapper.Map<Disclaimer>(entity);
        }

        public async Task<IDisclaimer> InsertAsync(IDisclaimer disclaimer)
        {
            var entity = new DisclaimerEntity(GetPartitionKey(disclaimer.LykkeEntityId), GetRowKey());
            Mapper.Map(disclaimer, entity);

            await _storage.InsertAsync(entity);

            var index = AzureIndex.Create(GetDisclaimerIdIndexPartitionKey(entity.RowKey),
                GetDisclaimerIdIndexRowKey(), entity);
            
            await _disclaimerIdIndexStorage.InsertAsync(index);

            return Mapper.Map<Disclaimer>(entity);
        }

        public async Task UpdateAsync(IDisclaimer disclaimer)
        {
            await _storage.MergeAsync(GetPartitionKey(disclaimer.LykkeEntityId), GetRowKey(disclaimer.Id), entity =>
            {
                Mapper.Map(disclaimer, entity);
                return entity;
            });
        }

        public async Task DeleteAsync(string lykkeEntityId, string disclaimerId)
        {
            await _storage.DeleteAsync(GetPartitionKey(lykkeEntityId), GetRowKey(disclaimerId));
            await _disclaimerIdIndexStorage
                .DeleteAsync(GetDisclaimerIdIndexPartitionKey(disclaimerId), GetDisclaimerIdIndexRowKey());
        }
        
        private static string GetPartitionKey(string lykkeEntityId)
            => lykkeEntityId;

        private static string GetRowKey(string disclaimerId)
            => disclaimerId;

        private static string GetRowKey()
            => Guid.NewGuid().ToString("D");
        
        private static string GetDisclaimerIdIndexPartitionKey(string disclaimerId)
            => disclaimerId;

        private static string GetDisclaimerIdIndexRowKey()
            => "DisclaimerLykkeEntityIndex";
    }
}
