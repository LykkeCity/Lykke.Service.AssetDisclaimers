using System;

namespace Lykke.Service.AssetDisclaimers.Core.Exceptions
{
    public class DisclaimerNotFoundException : Exception
    {
        public DisclaimerNotFoundException()
        {
        }

        public DisclaimerNotFoundException(string disclaimerId)
            : base("Disclaimer not found")
        {
            DisclaimerId = disclaimerId;
        }
        
        public string DisclaimerId { get; }
    }
}
