using System;

namespace Lykke.Service.AssetDisclaimers.Core.Exceptions
{
    public class LykkeEntityAlreadyExistsException : Exception
    {
        public LykkeEntityAlreadyExistsException()
        {
        }

        public LykkeEntityAlreadyExistsException(string lykkeEntityId)
            : base("Lykke entity already exists")
        {
            LykkeEntityId = lykkeEntityId;
        }
        
        public string LykkeEntityId { get; }
    }
}
