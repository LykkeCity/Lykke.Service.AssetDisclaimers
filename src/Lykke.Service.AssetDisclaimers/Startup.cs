using System;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.AzureStorage.Tables.Entity.Metamodel;
using Lykke.AzureStorage.Tables.Entity.Metamodel.Providers;
using Lykke.Sdk;
using Lykke.Service.AssetDisclaimers.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.AssetDisclaimers
{
    [UsedImplicitly]
    public class Startup
    {
        private readonly LykkeSwaggerOptions _swaggerOptions = new LykkeSwaggerOptions
        {
            ApiTitle = "AssetDisclaimers API",
            ApiVersion = "v1"
        };

        [UsedImplicitly]
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            EntityMetamodel.Configure(new AnnotationsBasedMetamodelProvider());

            Mapper.Initialize(cfg =>
            {
                cfg.AddProfiles(typeof(AzureRepositories.AutoMapperProfile));
                cfg.AddProfiles(typeof(AutoMapperProfile));
            });

            Mapper.AssertConfigurationIsValid();
            
            return services.BuildServiceProvider<AppSettings>(options =>
            {
                options.SwaggerOptions = _swaggerOptions;

                options.Logs = logs =>
                {
                    logs.AzureTableName = "AssetDisclaimersLog";
                    logs.AzureTableConnectionStringResolver = settings => settings.AssetDisclaimersService.Db.LogsConnectionString;
                };
            });
        }

        [UsedImplicitly]
        public void Configure(IApplicationBuilder app)
        {
            app.UseLykkeConfiguration(options =>
            {
                options.SwaggerOptions = _swaggerOptions;
            });
        }
    }
}
