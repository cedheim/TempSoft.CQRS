using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Theatres.Enums;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Commands
{
    public class AddAuditoriumProperty : EntityCommandBase
    {
        private AddAuditoriumProperty()
        {
        }

        public AddAuditoriumProperty(Guid entityId, AuditoriumProperties auditoriumProperty)
        {
            AuditoriumProperty = auditoriumProperty;
            EntityId = entityId;
        }

        public AuditoriumProperties AuditoriumProperty { get; }
    }
}