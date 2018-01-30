using System;

namespace Lykke.Service.AssetDisclaimers.Client 
{
    public class AssetDisclaimersServiceClientSettings 
    {
        public AssetDisclaimersServiceClientSettings()
        {
        }
        
        public AssetDisclaimersServiceClientSettings(string serviceUrl)
        {
            ServiceUrl = serviceUrl;
        }

        public string ServiceUrl {get; set;}
    }
}
