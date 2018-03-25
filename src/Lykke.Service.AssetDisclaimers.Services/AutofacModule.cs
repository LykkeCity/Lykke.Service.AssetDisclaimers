﻿using System;
using Autofac;
using Lykke.Service.AssetDisclaimers.Core.Services;

namespace Lykke.Service.AssetDisclaimers.Services
{
    public class AutofacModule : Module
    {
        private readonly TimeSpan _pendingTimeout;

        public AutofacModule(TimeSpan pendingTimeout)
        {
            _pendingTimeout = pendingTimeout;
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
                .WithParameter(TypedParameter.From(_pendingTimeout));
        }
    }
}
