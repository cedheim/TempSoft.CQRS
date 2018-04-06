using System;

namespace TempSoft.CQRS.Commands
{
    public abstract class EntityCommandBase : CommandBase, IEntityCommand
    {
        protected EntityCommandBase()
        {
        }

        public Guid EntityId { get; set; }
    }
}