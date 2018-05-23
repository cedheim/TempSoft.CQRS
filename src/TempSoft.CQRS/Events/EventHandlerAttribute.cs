using System;

namespace TempSoft.CQRS.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventHandlerAttribute : Attribute
    {
        public EventHandlerAttribute(Type @for)
        {
            For = @for;
        }

        public Type For { get; }
    }
}