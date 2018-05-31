using System;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Events
{
    public class TheatreInitialized : EventBase, IInitializationEvent
    {
        private TheatreInitialized() { }

        public TheatreInitialized(Guid aggregateRootId, string name)
        {
            AggregateRootId = aggregateRootId;
            Name = name;
        }

        public string Name { get; private set; }
    }
}