using System;

namespace NCG.NGS.CQRS.Events
{
    public interface IEvent
    {
        Guid Id { get; }

        int Version { get; set; }

        Guid AggregateRootId { get; set; }
    }
}