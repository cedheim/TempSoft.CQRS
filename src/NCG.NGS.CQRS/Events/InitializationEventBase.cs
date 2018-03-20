using System;

namespace NCG.NGS.CQRS.Events
{
    public abstract class InitializationEventBase : EventBase, IInitializationEvent
    {
        protected InitializationEventBase()
        {
        }

        protected InitializationEventBase(Guid aggregateRootId)
        {
            AggregateRootId = aggregateRootId;
        }
    }
}