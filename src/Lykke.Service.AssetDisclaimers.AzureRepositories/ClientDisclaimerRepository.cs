using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Core.Repositories;

namespace Lykke.Service.AssetDisclaimers.AzureRepositories
{
    public class ClientDisclaimerRepository : IClientDisclaimerRepository
    {
        private readonly INoSQLTableStorage<ClientDisclaimerEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _disclaimerIdIndexStorage;

        public ClientDisclaimerRepository(
            INoSQLTableStorage<ClientDisclaimerEntity> storage,
            INoSQLTableStorage<AzureIndex> disclaimerIdIndexStorage)
        {
            _storage = storage;
            _disclaimerIdIndexStorage = disclaimerIdIndexStorage;
        }
        
        public async Task<IReadOnlyList<IClientDisclaimer>> GetAsync(string clientId)
        {
            IEnumerable<ClientDisclaimerEntity> entities = await _storage.GetDataAsync(GetPartitionKey(clientId));

            return Mapper.Map<List<ClientDisclaimer>>(entities);
        }

        public async Task<IReadOnlyList<IClientDisclaimer>> FindAsync(string disclaimerId)
        {
            IEnumerable<AzureIndex> indexes = await _disclaimerIdIndexStorage
                .GetDataAsync(GetDisclaimerIdIndexPartitionKey(disclaimerId));

            IEnumerable<ClientDisclaimerEntity> entities = await _storage.GetDataAsync(indexes);
            
            return Mapper.Map<List<ClientDisclaimer>>(entities);
        }

        public async Task InsertOrReplaceAsync(IClientDisclaimer clientDisclaimer)
        {
            var entity = new ClientDisclaimerEntity(GetPartitionKey(clientDisclaimer.ClientId),
                GetRowKey(clientDisclaimer.DisclaimerId));
            Mapper.Map(clientDisclaimer, entity);

            await _storage.InsertOrReplaceAsync(entity);

            var index = AzureIndex.Create(GetDisclaimerIdIndexPartitionKey(clientDisclaimer.DisclaimerId),
                GetDisclaimerIdIndexRowKey(clientDisclaimer.ClientId), entity);
            
            await _disclaimerIdIndexStorage.InsertAsync(index);
        }

        public async Task DeleteAsync(string clientId, string disclaimerId)
        {
            await _storage.DeleteAsync(GetPartitionKey(clientId), GetRowKey(disclaimerId));
            await _disclaimerIdIndexStorage
                .DeleteAsync(GetDisclaimerIdIndexPartitionKey(disclaimerId), GetDisclaimerIdIndexRowKey(clientId));
        }
        
        private static string GetPartitionKey(string clientId)
            => clientId;

        private static string GetRowKey(string disclaimerId)
            => disclaimerId;

        private static string GetDisclaimerIdIndexPartitionKey(string disclaimerId)
            => disclaimerId;

        private static string GetDisclaimerIdIndexRowKey(string clientId)
            => $"DisclaimerClientsIndex_{clientId}";
    }
}
