using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.AssetDisclaimers.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class LykkeEntityEntity : AzureTableEntity
    {
        private int _priority;
        
        public LykkeEntityEntity()
        {
        }

        public LykkeEntityEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public int Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                MarkValueTypePropertyAsDirty(nameof(Priority));
            }
        }
    }
}
