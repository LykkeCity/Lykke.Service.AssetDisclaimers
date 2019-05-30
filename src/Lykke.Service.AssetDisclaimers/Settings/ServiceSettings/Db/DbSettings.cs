using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.AssetDisclaimers.Settings.ServiceSettings.Db
{
    [UsedImplicitly]
    public class DbSettings
    {
        [AzureTableCheck]
        public string DataConnectionString { get; set; }
        
        [AzureTableCheck]
        public string LogsConnectionString { get; set; }
    }
}
