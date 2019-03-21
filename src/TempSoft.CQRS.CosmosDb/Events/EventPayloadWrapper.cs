using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TempSoft.CQRS.CosmosDb.Common;
using TempSoft.CQRS.CosmosDb.Extensions;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.CosmosDb.Events
{
    public class EventPayloadWrapper : StorageBase
    {
        public const string DocumentTypeName = "event";

        [JsonConstructor]
        private EventPayloadWrapper()
        {
        }

        public EventPayloadWrapper(IEvent @event)
            : base(@event.AggregateRootId, DocumentTypeName)
        {
            Id = CreateIdentifier(@event.Id);
            EventId = @event.Id;
            Payload = JObject.FromObject(@event);
            PayloadType = @event.GetType().ToFriendlyName();
            Version = @event.Version;
            EventGroup = @event.EventGroup;
        }
        
        public Guid EventId { get; set; }

        public JObject Payload { get; set; }

        public string PayloadType { get; set; }

        public int Version { get; set; }

        public string EventGroup { get; set; }
        
        public IEvent GetEvent()
        {
            var type = Type.GetType(PayloadType);

            return (IEvent) Payload.ToObject(type);
        }

        public static string CreateIdentifier(Guid commandId)
        {
            return IdentityFormatter.Format(commandId.ToString(), DocumentTypeName);
        }
    }
}