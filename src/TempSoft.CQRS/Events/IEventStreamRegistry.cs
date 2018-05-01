using System.Collections.Generic;

namespace TempSoft.CQRS.Events
{
    public interface IEventStreamRegistry
    {
        void RegisterEventStream(EventStreamDefinition definition);

        IEnumerable<EventStreamDefinition> GetEventStreamsByEvent(IEvent @event);

        EventStreamDefinition GetEventStreamByName(string name);
    }
}