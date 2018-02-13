using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.AssetDisclaimers.Settings.ServiceSettings.Db
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string DataConnectionString { get; set; }
        
        [AzureTableCheck]
        public string LogsConnectionString { get; set; }
    }
}
