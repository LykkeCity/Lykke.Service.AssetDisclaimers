﻿using Autofac;
using Common.Log;
using Lykke.Payments.EasyPaymentGateway.Client;
using Lykke.Service.AssetDisclaimers.Settings;

namespace Lykke.Service.AssetDisclaimers.Modules
{
    public class ClientsModule : Module
    {
        private readonly ILog _log;
        private readonly AppSettings _appSettings;

        public ClientsModule(ILog log, AppSettings appSettings)
        {
            _log = log;
            _appSettings = appSettings;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterEasyPaymentGatewayClient(_appSettings.EasyPaymentGatewayServiceClient);
        }
    }
}
