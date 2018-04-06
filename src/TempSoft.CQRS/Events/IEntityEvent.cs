using System;

namespace TempSoft.CQRS.Events
{
    public interface IEntityEvent : IEvent
    {
        Guid EntityId { get; set; }
    }
}