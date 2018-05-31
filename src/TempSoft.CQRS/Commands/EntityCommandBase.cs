using System;

namespace TempSoft.CQRS.Commands
{
    public abstract class EntityCommandBase : CommandBase, IEntityCommand
    {
        public Guid EntityId { get; set; }
    }
}