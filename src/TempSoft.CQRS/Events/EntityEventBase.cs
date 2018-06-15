using System;

namespace TempSoft.CQRS.Events
{
    public abstract class EntityEventBase : EventBase, IEntityEvent
    {
        public string EntityId { get; set; }
    }
}