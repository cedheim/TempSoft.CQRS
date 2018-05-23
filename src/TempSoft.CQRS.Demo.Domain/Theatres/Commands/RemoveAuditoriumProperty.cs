using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Theatres.Enums;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Commands
{
    public class RemoveAuditoriumProperty : EntityCommandBase
    {
        private RemoveAuditoriumProperty()
        {
        }

        public RemoveAuditoriumProperty(Guid entityId, AuditoriumProperties auditoriumProperty)
        {
            AuditoriumProperty = auditoriumProperty;
            EntityId = entityId;
        }

        public AuditoriumProperties AuditoriumProperty { get; }
    }
}