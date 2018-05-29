using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Events
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly ConcurrentDictionary<Guid, ConcurrentBag<IEvent>> _db = new ConcurrentDictionary<Guid, ConcurrentBag<IEvent>>();

        public Task<IEnumerable<IEvent>> Get(Guid id, int fromVersion = default(int), CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                var bag = _db.GetOrAdd(id, guid => new ConcurrentBag<IEvent>());

                return bag.Where(evnt => evnt.Version >= fromVersion).OrderBy(evnt => evnt.Version).AsEnumerable();
            }, cancellationToken);
        }

        public Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.Run(() =>
            {
                foreach (var @event in events)
                {
                    var bag = _db.GetOrAdd(@event.AggregateRootId, guid => new ConcurrentBag<IEvent>());
                    bag.Add(@event);
                }
            }, cancellationToken);
        }

        public async Task List(EventStoreFilter filter, Func<IEvent, CancellationToken, Task> callback, CancellationToken cancellationToken = default(CancellationToken))
        {
            var aggregateRootIds = filter.AggregateRootId.HasValue ? new[] {filter.AggregateRootId.Value} : _db.Keys.ToArray();
            foreach (var aggregateRootId in aggregateRootIds)
            {
                var bag = _db.GetOrAdd(aggregateRootId, guid => new ConcurrentBag<IEvent>());
                var query = bag.AsEnumerable();

                if (filter.EventGroups?.Length > 0)
                {
                    query = query.Where(e => filter.EventGroups.Contains(e.EventGroup));
                }

                if (filter.EventTypes?.Length > 0)
                {
                    query = query.Where(e => filter.EventTypes.Contains(e.GetType()));
                }

                foreach (var @event in query)
                {
                    await callback(@event, cancellationToken);
                }
            }
        }
    }
}