using System;
using TempSoft.CQRS.Demo.Domain.Movies.Enums;
using TempSoft.CQRS.Demo.Domain.Theatres.Enums;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Events
{
    public class AuditoriumPropertyRemoved : EntityEventBase
    {
        public AuditoriumProperties AuditoriumProperty { get; private set; }
        private AuditoriumPropertyRemoved() { }

        public AuditoriumPropertyRemoved(Guid entityId, AuditoriumProperties auditoriumProperty)
        {
            AuditoriumProperty = auditoriumProperty;
            EntityId = entityId;
        }
    }
}