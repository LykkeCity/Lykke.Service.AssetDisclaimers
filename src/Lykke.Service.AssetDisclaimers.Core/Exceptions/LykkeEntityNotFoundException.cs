using System;

namespace Lykke.Service.AssetDisclaimers.Core.Exceptions
{
    public class LykkeEntityNotFoundException : Exception
    {
        public LykkeEntityNotFoundException()
        {
        }

        public LykkeEntityNotFoundException(string lykkeEntityId)
            : base("Lykke entity not found")
        {
            LykkeEntityId = lykkeEntityId;
        }
        
        public string LykkeEntityId { get; }
    }
}
