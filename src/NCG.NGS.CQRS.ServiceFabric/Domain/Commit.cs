using System;
using System.Collections.Generic;
using System.Linq;
using NCG.NGS.CQRS.Events;

namespace NCG.NGS.CQRS.ServiceFabric.Domain
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