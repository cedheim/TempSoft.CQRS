using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TempSoft.CQRS.Exceptions;

namespace TempSoft.CQRS.Events
{
    public class EventStreamRegistry : IEventStreamRegistry, IEnumerable<EventStreamDefinition>
    {
        private HashSet<EventStreamDefinition> _definitions;

        public EventStreamRegistry()
        {
            _definitions = new HashSet<EventStreamDefinition>();
        }

        public EventStreamRegistry(IEnumerable<EventStreamDefinition> definitions)
        {
            _definitions = new HashSet<EventStreamDefinition>(definitions);
        }

        public void RegisterEventStream(EventStreamDefinition definition)
        {
            if (!_definitions.Add(definition))
            {
                throw new DuplicateEventStreamDefinitionException($"Unable to add definition {definition.Name}, it already exists.");
            }
        }

        public IEnumerable<EventStreamDefinition> GetEventStreamsByEvent(IEvent @event)
        {
            return _definitions.Where(definition => definition.Filter.Match(@event));
        }

        public EventStreamDefinition GetEventStreamByName(string name)
        {
            return _definitions.FirstOrDefault(def => def.Name == name);
        }


        public IEnumerator<EventStreamDefinition> GetEnumerator()
        {
            return _definitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}