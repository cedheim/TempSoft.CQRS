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
        private EventPayloadWrapper() { }

        public EventPayloadWrapper(IEvent @event)
        {
            Id = @event.Id;
            Payload = JObject.FromObject(@event);
            PayloadType = @event.GetType().ToFriendlyName();
            Version = @event.Version;
            Timestamp = @event.Timestamp;
            AggregateRootId = @event.AggregateRootId;
        }

        [JsonProperty("id")]
        public Guid Id { get; set; }

        public JObject Payload { get; set; }

        public string PayloadType { get; set; }

        public int Version { get; set; }

        public Guid AggregateRootId { get; set; }

        public DateTime Timestamp { get; set; }

        public IEvent GetEvent()
        {
            var type = Type.GetType(PayloadType);

            return (IEvent)Payload.ToObject(type);
        }
    }
}