using System;

namespace TempSoft.CQRS.Events
{
    public interface IEvent
    {
        Guid Id { get; }

        DateTime Timestamp { get; }

        int Version { get; set; }

        Guid AggregateRootId { get; set; }
    }
}