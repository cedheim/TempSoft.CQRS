using System;

namespace TempSoft.CQRS.Commands
{
    public interface ICommand
    {
        Guid Id { get; }

        DateTime Timestamp { get; }
    }
}