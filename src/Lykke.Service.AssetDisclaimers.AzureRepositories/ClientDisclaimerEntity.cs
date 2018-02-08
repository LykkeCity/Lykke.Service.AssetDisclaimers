using System;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Annotation;
using Lykke.AzureStorage.Tables.Entity.ValueTypesMerging;

namespace Lykke.Service.AssetDisclaimers.AzureRepositories
{
    [ValueTypeMergingStrategy(ValueTypeMergingStrategy.UpdateIfDirty)]
    public class ClientDisclaimerEntity : AzureTableEntity
    {
        private bool _approved;
        
        public ClientDisclaimerEntity()
        {
        }

        public ClientDisclaimerEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
        
        public string ClientId { get; set; }
        
        public string DisclaimerId { get; set; }
        
        public bool Approved
        {
            get => _approved;
            set
            {
                _approved = value;
                MarkValueTypePropertyAsDirty(nameof(Approved));
            }
        }
        
        public DateTime? ApprovedDate { get; set; }
    }
}
