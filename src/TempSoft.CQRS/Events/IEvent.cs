using System;

namespace TempSoft.CQRS.Events
{
    public interface IEvent
    {
        Guid Id { get; }

        DateTime Timestamp { get; }

        int Version { get; set; }

        string AggregateRootId { get; set; }

        string EventGroup { get; set; }
    }
}