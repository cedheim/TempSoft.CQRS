using System;

namespace NCG.NGS.CQRS.Events
{
    public interface IEvent
    {
        Guid Id { get; }

        DateTime Timestamp { get; }

        int Version { get; set; }

        Guid AggregateRootId { get; set; }
    }
}