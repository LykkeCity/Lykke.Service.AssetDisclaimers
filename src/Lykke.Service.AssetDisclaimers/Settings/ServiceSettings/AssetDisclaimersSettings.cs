using System;
using JetBrains.Annotations;
using Lykke.Service.AssetDisclaimers.Settings.ServiceSettings.Db;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.AssetDisclaimers.Settings.ServiceSettings
{
    [UsedImplicitly]
    public class AssetDisclaimersSettings
    {
        public DbSettings Db { get; set; }
        public RedisSettings Redis { get; set; }

        public TimeSpan PendingTimeout { get; set; }

        [Optional]
        public string DepositDelayDisclaimerId { get; set; }
    }
}
