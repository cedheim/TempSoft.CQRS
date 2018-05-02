using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace TempSoft.CQRS.CosmosDb.Commands
{
    public class CommandRegistryWrapper
    {
        private CommandRegistryWrapper() { }

        public CommandRegistryWrapper(Guid aggregateRootId, Guid commandId)
        {
            Id = commandId;
            AggregateRootId = aggregateRootId;
        }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        public Guid AggregateRootId { get; set; }

        [JsonProperty("_ts")]
        public long Timestamp { get; set; }
    }
}