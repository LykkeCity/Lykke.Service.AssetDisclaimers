using System;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using StackExchange.Redis;

namespace Lykke.Service.AssetDisclaimers.Services.Extensions
{
    public static class RedisExtensions
    {
        private const string DateFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        
        public static IDisclaimer ToDisclaimer(this HashEntry[] hashEntry)
        {
            var disclaimer = new Disclaimer();
            
            var hashDict = hashEntry.ToDictionary();

            if (hashDict.TryGetValue(nameof(disclaimer.Id), out var id))
                disclaimer.Id = id;
            
            if (hashDict.TryGetValue(nameof(disclaimer.LykkeEntityId), out var lykkeEntityId))
                disclaimer.LykkeEntityId = lykkeEntityId;
            
            if (hashDict.TryGetValue(nameof(disclaimer.Type), out var type))
                disclaimer.Type = Enum.Parse<DisclaimerType>(type);

            if (hashDict.TryGetValue(nameof(disclaimer.Name), out var name))
                disclaimer.Name = name;
            
            if (hashDict.TryGetValue(nameof(disclaimer.Text), out var text))
                disclaimer.Text = text;
            
            if (hashDict.TryGetValue(nameof(disclaimer.StartDate), out var startDate))
                disclaimer.StartDate = DateTime.Parse(startDate).ToUniversalTime();
            
            if (hashDict.TryGetValue(nameof(disclaimer.ShowOnEachAction), out var showOnEachAction))
                disclaimer.ShowOnEachAction = Convert.ToBoolean(showOnEachAction);
            
            return disclaimer;
        }
        
        public static HashEntry[] ToHashEntries(this IDisclaimer disclaimer)
        {
            return new[]
            {
                new HashEntry(nameof(Disclaimer.Id), disclaimer.Id),
                new HashEntry(nameof(Disclaimer.LykkeEntityId), disclaimer.LykkeEntityId ?? string.Empty),
                new HashEntry(nameof(Disclaimer.Type), disclaimer.Type.ToString() ?? string.Empty),
                new HashEntry(nameof(Disclaimer.Name), disclaimer.Name ?? string.Empty),
                new HashEntry(nameof(Disclaimer.Text), disclaimer.Text ?? string.Empty),
                new HashEntry(nameof(Disclaimer.StartDate), disclaimer.StartDate.ToString(DateFormat)),
                new HashEntry(nameof(Disclaimer.ShowOnEachAction), disclaimer.ShowOnEachAction.ToString())
            };
        }
        
        public static ILykkeEntity ToLykkeEntity(this HashEntry[] hashEntry)
        {
            var entity = new LykkeEntity();
            
            var hashDict = hashEntry.ToDictionary();

            if (hashDict.TryGetValue(nameof(entity.Id), out var id))
                entity.Id = id;
            
            if (hashDict.TryGetValue(nameof(entity.Name), out var name))
                entity.Name = name;
            
            if (hashDict.TryGetValue(nameof(entity.Description), out var description))
                entity.Description = description;

            if (hashDict.TryGetValue(nameof(entity.Priority), out var priority))
                entity.Priority = Convert.ToInt32(priority);
            
            return entity;
        }
        
        public static HashEntry[] ToHashEntries(this ILykkeEntity entity)
        {
            return new[]
            {
                new HashEntry(nameof(LykkeEntity.Id), entity.Id),
                new HashEntry(nameof(LykkeEntity.Name), entity.Name ?? string.Empty),
                new HashEntry(nameof(LykkeEntity.Description), entity.Description ?? string.Empty),
                new HashEntry(nameof(LykkeEntity.Priority), entity.Priority.ToString() ?? string.Empty),
            };
        }
        
        public static IClientDisclaimer ToClientDisclaimer(this HashEntry[] hashEntry)
        {
            var disclaimer = new ClientDisclaimer();
            
            var hashDict = hashEntry.ToDictionary();

            if (hashDict.TryGetValue(nameof(disclaimer.ClientId), out var clientId))
                disclaimer.ClientId = clientId;
            
            if (hashDict.TryGetValue(nameof(disclaimer.DisclaimerId), out var disclaimerId))
                disclaimer.DisclaimerId = disclaimerId;
            
            if (hashDict.TryGetValue(nameof(disclaimer.Approved), out var approved))
                disclaimer.Approved = Convert.ToBoolean(approved);
            
            if (hashDict.TryGetValue(nameof(disclaimer.ApprovedDate), out var approvedDate))
                disclaimer.ApprovedDate = DateTime.Parse(approvedDate).ToUniversalTime();
            
            return disclaimer;
        }
        
        public static HashEntry[] ToHashEntries(this IClientDisclaimer disclaimer)
        {
            return new[]
            {
                new HashEntry(nameof(ClientDisclaimer.ClientId), disclaimer.ClientId),
                new HashEntry(nameof(ClientDisclaimer.DisclaimerId), disclaimer.DisclaimerId ?? string.Empty),
                new HashEntry(nameof(ClientDisclaimer.Approved), disclaimer.Approved.ToString() ?? string.Empty),
                new HashEntry(nameof(ClientDisclaimer.ApprovedDate), disclaimer.ApprovedDate.ToString(DateFormat))
            };
        }
    }
}
