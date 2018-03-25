using System;
using Lykke.Service.AssetDisclaimers.Settings.ServiceSettings.Db;

namespace Lykke.Service.AssetDisclaimers.Settings.ServiceSettings
{
    public class AssetDisclaimersSettings
    {
        public DbSettings Db { get; set; }

        public TimeSpan PendingTimeout { get; set; }
    }
}
