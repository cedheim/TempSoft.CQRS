using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TempSoft.CQRS.CosmosDb.Extensions;

namespace TempSoft.CQRS.CosmosDb.Common
{
    public abstract class StorageBase
    {
        protected StorageBase()
        {
        }

        protected StorageBase(string partitionId, string documentType)
        {
            PartitionId = partitionId;
            DocumentType = documentType;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        public string PartitionId { get; set; }

        public string DocumentType { get; set; }
        
        [JsonProperty("_ts")]
        public long Epoch { get; set; }
        
        [JsonIgnore]
        public DateTime Timestamp => Epoch.ToDateTime();
    }
}