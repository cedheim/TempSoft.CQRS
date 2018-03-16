using System;

namespace NCG.NGS.CQRS.Events
{
    public class InitializationEvent : IEvent
    {
        private InitializationEvent()
        {
        }

        public InitializationEvent(Guid aggregateRootId)
        {
            Id = Guid.NewGuid();
            AggregateRootId = aggregateRootId;
        }

        public Guid Id { get; private set; }
        public int Version { get; set; }
        public Guid AggregateRootId { get; set; }
    }
}