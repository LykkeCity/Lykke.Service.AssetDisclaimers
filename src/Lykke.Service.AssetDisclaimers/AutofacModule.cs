using Autofac;
using Common.Log;

namespace Lykke.Service.AssetDisclaimers
{
    public class AutofacModule : Module
    {
        private readonly ILog _log;

        public AutofacModule(ILog log)
        {
            _log = log;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();
        }
    }
}
