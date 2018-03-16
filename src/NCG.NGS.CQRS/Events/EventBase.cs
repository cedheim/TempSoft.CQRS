using System;

namespace NCG.NGS.CQRS.Events
{
    public abstract class EventBase : IEvent
    {
        protected EventBase()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
        public int Version { get; set; }
        public Guid AggregateRootId { get; set; }
    }
}