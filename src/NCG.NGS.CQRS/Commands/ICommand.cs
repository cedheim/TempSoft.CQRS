using System;

namespace NCG.NGS.CQRS.Commands
{
    public interface ICommand
    {
        Guid Id { get; }
    }
}