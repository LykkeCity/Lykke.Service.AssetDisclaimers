using System;
using Autofac;
using Lykke.Service.AssetDisclaimers.Core.Services;
using Lykke.Service.AssetDisclaimers.Services;
using Lykke.Service.AssetDisclaimers.Settings.ServiceSettings;

namespace Lykke.Service.AssetDisclaimers.Modules
{
    public class ServicesModule : Module
    {
        private readonly AssetDisclaimersSettings _settings;

        public ServicesModule(AssetDisclaimersSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();
            
            builder.RegisterType<DisclaimerService>()
                .As<IDisclaimerService>();
            
            builder.RegisterType<LykkeEntityService>()
                .As<ILykkeEntityService>();
            
            builder.RegisterType<ClientDisclaimerService>()
                .As<IClientDisclaimerService>()
                .WithParameter(TypedParameter.From(_settings.PendingTimeout))
                .WithParameter(TypedParameter.From(_settings.DepositDelayDisclaimerId));
        }
    }
}
