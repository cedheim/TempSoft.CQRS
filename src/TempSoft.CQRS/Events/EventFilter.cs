using System;
using System.Linq;

namespace TempSoft.CQRS.Events
{
    public class EventFilter
    {
        public string[] EventGroups { get; set; }

        public Type[] EventTypes { get; set; }

        public bool Match(IEvent e)
        {
            var matchesEventGroup = (EventGroups?.Length ?? 0) == 0 || EventGroups.Contains(e.EventGroup);
            var matchesEventTypes = (EventTypes?.Length ?? 0) == 0 || EventTypes.Contains(e.GetType());

            return matchesEventTypes && matchesEventGroup;
        }
    }
}