using Autofac;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using Common.Log;
using Lykke.Service.AssetDisclaimers.Core.Repositories;
using Lykke.SettingsReader;

namespace Lykke.Service.AssetDisclaimers.AzureRepositories
{
    public class AutofacModule : Module
    {
        private readonly IReloadingManager<string> _connectionString;
        private readonly ILog _log;

        public AutofacModule(IReloadingManager<string> connectionString, ILog log)
        {
            _connectionString = connectionString;
            _log = log;
        }

        protected override void Load(ContainerBuilder builder)
        {
            const string disclaimersTableName = "Disclaimers";
            const string clientDisclaimersTableName = "ClientDisclaimers";

            builder.RegisterInstance<ILykkeEntityRepository>(new LykkeEntityRepository(
                AzureTableStorage<LykkeEntityEntity>.Create(_connectionString,
                    disclaimersTableName, _log)));

            builder.RegisterInstance<IDisclaimerRepository>(new DisclaimerRepository(
                AzureTableStorage<DisclaimerEntity>.Create(_connectionString,
                    disclaimersTableName, _log),
                AzureTableStorage<AzureIndex>.Create(_connectionString,
                    disclaimersTableName, _log)));
            
            builder.RegisterInstance<IClientDisclaimerRepository>(new ClientDisclaimerRepository(
                AzureTableStorage<ClientDisclaimerEntity>.Create(_connectionString,
                    clientDisclaimersTableName, _log),
                AzureTableStorage<AzureIndex>.Create(_connectionString,
                    clientDisclaimersTableName, _log)));
        }
    }
}
