using System;

namespace NCG.NGS.CQRS.Events
{
    public abstract class EventBase : IEvent
    {
        protected EventBase()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public int Version { get; set; }
        public Guid AggregateRootId { get; set; }
    }
}