using System;
using System.Collections.Generic;
using System.Linq;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Domain
{
    public class Commit
    {
        public Commit(Guid aggregateRootId, IEnumerable<IEvent> events, IEnumerable<Guid> commandIds)
        {
            AggregateRootId = aggregateRootId;
            Events = events?.ToArray() ?? new IEvent[0];
            CommandIds = commandIds?.ToArray() ?? new Guid[0];
        }

        public Guid AggregateRootId { get; }

        public IEvent[] Events { get; }

        public Guid[] CommandIds { get; }
    }
}