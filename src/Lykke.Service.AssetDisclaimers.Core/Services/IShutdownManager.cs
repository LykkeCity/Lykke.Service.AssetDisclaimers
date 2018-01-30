using System.Threading.Tasks;

namespace Lykke.Service.AssetDisclaimers.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}