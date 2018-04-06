using System;

namespace TempSoft.CQRS.Commands
{
    public interface IEntityCommand : ICommand
    {
        Guid EntityId { get; set; }
    }
}