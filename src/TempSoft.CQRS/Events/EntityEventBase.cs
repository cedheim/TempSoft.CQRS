using System;

namespace TempSoft.CQRS.Events
{
    public abstract class EntityEventBase : EventBase, IEntityEvent
    {
        public Guid EntityId { get; set; }
    }
}