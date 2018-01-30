using System;
using System.Net.Http;
using Microsoft.Extensions.PlatformAbstractions;

namespace Lykke.Service.AssetDisclaimers.Client
{
    public class AssetDisclaimersClient : IAssetDisclaimersClient, IDisposable
    {
        private readonly HttpClient _httpClient;
        
        public AssetDisclaimersClient(AssetDisclaimersServiceClientSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if (string.IsNullOrEmpty(settings.ServiceUrl))
                throw new ArgumentException("Service URL Required");
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(settings.ServiceUrl),
                DefaultRequestHeaders =
                {
                    {
                        "User-Agent",
                        $"{PlatformServices.Default.Application.ApplicationName}/{PlatformServices.Default.Application.ApplicationVersion}"
                    }
                }
            };
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
