using System;

namespace NCG.NGS.CQRS.Events
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventHandlerAttribute : Attribute
    {
        public Type For { get; }

        public EventHandlerAttribute(Type @for)
        {
            For = @for;
        }
        
    }
}