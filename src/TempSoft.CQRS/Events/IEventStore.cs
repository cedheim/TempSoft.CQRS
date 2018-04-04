using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TempSoft.CQRS.Events
{
    public interface IEventStore
    {
        Task<IEnumerable<IEvent>> Get(Guid id, int fromVersion = default(int), CancellationToken cancellationToken = default(CancellationToken));

        Task Save(Guid id, IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken));
    }
}