using System;

namespace TempSoft.CQRS.Projectors
{

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class ProjectorAttribute : Attribute
    {
        public ProjectorAttribute(Type @for)
        {
            For = @for;
        }

        public Type For { get; }
    }
}