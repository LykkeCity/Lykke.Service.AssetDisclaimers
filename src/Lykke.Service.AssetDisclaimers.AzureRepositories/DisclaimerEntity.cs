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
        private DisclaimerType _type;
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
        
        public DisclaimerType Type
        {
            get => _type;
            set
            {
                _type = value;
                MarkValueTypePropertyAsDirty(nameof(Type));
            }
        }

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

        public bool ShowOnEachAction { get; set; }
    }
}
