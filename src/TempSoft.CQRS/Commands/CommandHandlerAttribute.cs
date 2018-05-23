using System;

namespace TempSoft.CQRS.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandHandlerAttribute : Attribute
    {
        public CommandHandlerAttribute(Type @for)
        {
            For = @for;
        }

        public Type For { get; }
    }
}