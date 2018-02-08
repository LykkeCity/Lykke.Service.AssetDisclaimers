namespace Lykke.Service.AssetDisclaimers.Client 
{
    /// <summary>
    /// Settings for <see cref="IAssetDisclaimersClient"/>.
    /// </summary>
    public class AssetDisclaimersServiceClientSettings 
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AssetDisclaimersServiceClientSettings"/>.
        /// </summary>
        public AssetDisclaimersServiceClientSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AssetDisclaimersServiceClientSettings"/> with service url.
        /// </summary>
        public AssetDisclaimersServiceClientSettings(string serviceUrl)
        {
            ServiceUrl = serviceUrl;
        }

        /// <summary>
        /// The asset disclaimers service url.
        /// </summary>
        public string ServiceUrl {get; set;}
    }
}
