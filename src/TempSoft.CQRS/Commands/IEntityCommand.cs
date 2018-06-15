using System;

namespace TempSoft.CQRS.Commands
{
    public interface IEntityCommand : ICommand
    {
        string EntityId { get; set; }
    }
}