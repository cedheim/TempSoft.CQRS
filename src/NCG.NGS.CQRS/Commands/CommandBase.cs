using System;

namespace NCG.NGS.CQRS.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected CommandBase()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; private set; }
    }
}