using System;

namespace NCG.NGS.CQRS.Events
{
    public class InitializationEvent : EventBase
    {
        private InitializationEvent()
        {
        }

        public InitializationEvent(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }
    }
}