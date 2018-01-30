using Lykke.Service.AssetDisclaimers.Settings.ServiceSettings;
using Lykke.Service.AssetDisclaimers.Settings.SlackNotifications;

namespace Lykke.Service.AssetDisclaimers.Settings
{
    public class AppSettings
    {
        public AssetDisclaimersSettings AssetDisclaimersService { get; set; }
        public SlackNotificationsSettings SlackNotifications { get; set; }
    }
}
