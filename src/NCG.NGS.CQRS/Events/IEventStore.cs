using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NCG.NGS.CQRS.Events
{
    public interface IEventStore
    {
        Task<IEnumerable<IEvent>> Get(Guid id, int fromVersion = default(int), CancellationToken cancellationToken = default(CancellationToken));

        Task Save(IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken));
    }
}