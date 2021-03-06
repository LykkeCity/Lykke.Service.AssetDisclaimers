﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.AssetDisclaimers.Core.Domain;

namespace Lykke.Service.AssetDisclaimers.Core.Repositories
{
    public interface IDisclaimerRepository
    {
        Task<IReadOnlyList<IDisclaimer>> GetAllAsync();

        Task<IDisclaimer> FindAsync(string disclaimerId);
        
        Task<IDisclaimer> InsertAsync(IDisclaimer disclaimer);
        
        Task UpdateAsync(IDisclaimer disclaimer);

        Task DeleteAsync(string lykkeEntityId, string disclaimerId);
    }
}
