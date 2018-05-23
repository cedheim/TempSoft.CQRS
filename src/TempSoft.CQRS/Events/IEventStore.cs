using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Events
{
    public interface IEventStore
    {
        Task<IEnumerable<IEvent>> Get(Guid id, int fromVersion = default(int),
            CancellationToken cancellationToken = default(CancellationToken));

        Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken));

        Task List(EventStoreFilter filter, Func<IEvent, CancellationToken, Task> callback,
            CancellationToken cancellationToken = default(CancellationToken));
    }

    public class EventStoreFilter
    {
        public string[] EventGroups { get; set; }
        public Guid? AggregateRootId { get; set; }
        public string[] EventTypes { get; set; }
        public DateTime? From { get; set; }
    }
}