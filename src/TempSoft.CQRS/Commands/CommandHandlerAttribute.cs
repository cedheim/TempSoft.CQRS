using System;

namespace TempSoft.CQRS.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandHandlerAttribute : Attribute
    {
        public Type For { get; }

        public CommandHandlerAttribute(Type @for)
        {
            For = @for;
        }
    }
}