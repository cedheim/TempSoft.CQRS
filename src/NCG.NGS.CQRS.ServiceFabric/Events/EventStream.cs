using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NCG.NGS.CQRS.Events;

namespace NCG.NGS.CQRS.ServiceFabric.Events
{
    [DataContract]
    public class EventStream : ICollection<IEvent>
    {
        [DataMember(Name = "States")]
        private readonly List<EventState> _states;

        public EventStream()
        {
            _states = new List<EventState>();
        }

        public EventStream(IEnumerable<IEvent> events)
        {
            _states = new List<EventState>(events.Select(e => new EventState(e)));
        }

        public IEnumerator<IEvent> GetEnumerator()
        {
            return _states.Select(state => state.Body).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(IEvent item)
        {
            _states.Add(new EventState(item));
        }

        public void AddRange(IEnumerable<IEvent> events)
        {
            _states.AddRange(events.Select(e => new EventState(e)));
        }

        public void Clear()
        {
            _states.Clear();
        }

        public bool Contains(IEvent item)
        {
            return _states.Any(state => state.Body.Version == item.Version);
        }

        public void CopyTo(IEvent[] array, int arrayIndex)
        {
            foreach (var state in _states)
            {
                if (arrayIndex >= array.Length)
                {
                    break;
                }

                array[arrayIndex] = state.Body;
                ++arrayIndex;
            }
        }

        public bool Remove(IEvent item)
        {
            var existing = _states.FirstOrDefault(state => state.Body.Version == item.Version);

            if (object.ReferenceEquals(existing, default(EventState)))
                return false;

            return _states.Remove(existing);
        }

        [IgnoreDataMember] public int Count => _states.Count;

        [IgnoreDataMember] public bool IsReadOnly => false;
    }
}