using System;

namespace TempSoft.CQRS.Events
{
    public interface IEntityEvent : IEvent
    {
        string EntityId { get; set; }
    }
}