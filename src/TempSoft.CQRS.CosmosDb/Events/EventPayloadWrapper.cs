using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.CosmosDb.Events
{
    public class EventPayloadWrapper
    {
        [JsonConstructor]
        private EventPayloadWrapper()
        {
        }

        public EventPayloadWrapper(IEvent @event)
        {
            Id = @event.Id;
            Payload = JObject.FromObject(@event);
            PayloadType = @event.GetType().ToFriendlyName();
            Version = @event.Version;
            AggregateRootId = @event.AggregateRootId;
            EventGroup = @event.EventGroup;
        }

        [JsonProperty("id")] public Guid Id { get; set; }

        public JObject Payload { get; set; }

        public string PayloadType { get; set; }

        public int Version { get; set; }

        public Guid AggregateRootId { get; set; }

        public string EventGroup { get; set; }

        [JsonProperty("_ts")] public long Timestamp { get; set; }

        public IEvent GetEvent()
        {
            var type = Type.GetType(PayloadType);

            return (IEvent) Payload.ToObject(type);
        }
    }
}