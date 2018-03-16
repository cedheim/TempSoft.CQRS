using System;

namespace NCG.NGS.CQRS.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected CommandBase()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
    }
}