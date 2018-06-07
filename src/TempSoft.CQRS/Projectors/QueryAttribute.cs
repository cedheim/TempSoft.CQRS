using System;

namespace TempSoft.CQRS.Projectors
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class QueryAttribute : Attribute
    {
        public QueryAttribute(Type @for)
        {
            For = @for;
        }

        public Type For { get; }
    }
}