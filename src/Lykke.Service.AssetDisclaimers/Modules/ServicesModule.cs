using System;
using Autofac;
using AzureStorage.Tables;
using AzureStorage.Tables.Templates.Index;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Payments.EasyPaymentGateway.Client;
using Lykke.Service.AssetDisclaimers.AzureRepositories;
using Lykke.Service.AssetDisclaimers.Core.Repositories;
using Lykke.Service.AssetDisclaimers.Core.Services;
using Lykke.Service.AssetDisclaimers.Services;
using Lykke.Service.AssetDisclaimers.Settings;
using Lykke.SettingsReader;
using StackExchange.Redis;

namespace Lykke.Service.AssetDisclaimers.Modules
{
    [UsedImplicitly]
    public class ServicesModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public ServicesModule(IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
                {
                    var lazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_settings.CurrentValue.AssetDisclaimersService.Redis.Configuration)); 
                    return lazy.Value;
                })
                .As<IConnectionMultiplexer>()
                .SingleInstance();

            builder.Register(c => c.Resolve<IConnectionMultiplexer>().GetDatabase())
                .As<IDatabase>();
            
            builder.RegisterType<RedisService>()
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterType<InitService>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
            
            const string disclaimersTableName = "Disclaimers";
            const string clientDisclaimersTableName = "ClientDisclaimers";

            IReloadingManager<string> connString = _settings.ConnectionString(x => x.AssetDisclaimersService.Db.DataConnectionString);

            builder.Register(ctx =>
                    new LykkeEntityRepository(
                        AzureTableStorage<LykkeEntityEntity>.Create(connString,
                            disclaimersTableName, ctx.Resolve<ILogFactory>())))
                .As<ILykkeEntityRepository>().SingleInstance();

            builder.Register(ctx => 
                new DisclaimerRepository(
                AzureTableStorage<DisclaimerEntity>.Create(connString,
                    disclaimersTableName, ctx.Resolve<ILogFactory>()),
                AzureTableStorage<AzureIndex>.Create(connString,
                    disclaimersTableName, ctx.Resolve<ILogFactory>())))
                .As<IDisclaimerRepository>().SingleInstance();
            
            builder.Register(ctx =>
                new ClientDisclaimerRepository(
                AzureTableStorage<ClientDisclaimerEntity>.Create(connString,
                    clientDisclaimersTableName, ctx.Resolve<ILogFactory>()),
                AzureTableStorage<AzureIndex>.Create(connString,
                    clientDisclaimersTableName, ctx.Resolve<ILogFactory>())))
                .As<IClientDisclaimerRepository>().SingleInstance();
            
            builder.RegisterType<DisclaimerService>()
                .As<IDisclaimerService>();
            
            builder.RegisterType<LykkeEntityService>()
                .As<ILykkeEntityService>();
            
            builder.RegisterType<ClientDisclaimerService>()
                .As<IClientDisclaimerService>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.AssetDisclaimersService.PendingTimeout))
                .WithParameter(TypedParameter.From(_settings.CurrentValue.AssetDisclaimersService.DepositDelayDisclaimerId));
            
            builder.RegisterEasyPaymentGatewayClient(_settings.CurrentValue.EasyPaymentGatewayServiceClient);
        }
    }
}
