using Lykke.AzureQueueIntegration;

namespace Lykke.Service.AssetDisclaimers.Settings.SlackNotifications
{
    public class SlackNotificationsSettings
    {
        public AzureQueueSettings AzureQueue { get; set; }
    }
}
