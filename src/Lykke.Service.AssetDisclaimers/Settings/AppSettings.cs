using JetBrains.Annotations;
using Lykke.Payments.FxPaygate.Client;
using Lykke.Sdk.Settings;
using Lykke.Service.AssetDisclaimers.Settings.ServiceSettings;

namespace Lykke.Service.AssetDisclaimers.Settings
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class AppSettings : BaseAppSettings
    {
        public AssetDisclaimersSettings AssetDisclaimersService { get; set; }
        public FxPaygateClientSettings FxPaygateServiceClient { get; set; }
    }
}
