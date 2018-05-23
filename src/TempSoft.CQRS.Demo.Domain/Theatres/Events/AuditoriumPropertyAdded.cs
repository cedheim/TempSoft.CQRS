using System;
using TempSoft.CQRS.Demo.Domain.Theatres.Enums;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Events
{
    public class AuditoriumPropertyAdded : EntityEventBase
    {
        private AuditoriumPropertyAdded()
        {
        }

        public AuditoriumPropertyAdded(Guid entityId, AuditoriumProperties auditoriumProperty)
        {
            AuditoriumProperty = auditoriumProperty;
            EntityId = entityId;
        }

        public AuditoriumProperties AuditoriumProperty { get; }
    }
}