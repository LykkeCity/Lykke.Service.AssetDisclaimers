using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;
using Lykke.Service.AssetDisclaimers.Core.Domain;

namespace Lykke.Service.AssetDisclaimers.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class DisclaimerEntity : AzureTableEntity
    {
        private DateTime _startDate;
        
        public DisclaimerEntity()
        {
        }

        public DisclaimerEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
        
        public string LykkeEntityId { get; set; }
        
        public DisclaimerType Type { get; set; }

        public string Name { get; set; }
        
        public string Text { get; set; }
        
        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                MarkValueTypePropertyAsDirty(nameof(StartDate));
            }
        }
    }
}
