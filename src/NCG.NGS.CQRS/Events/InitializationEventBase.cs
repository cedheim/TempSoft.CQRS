using System;

namespace TempSoft.CQRS.Events
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