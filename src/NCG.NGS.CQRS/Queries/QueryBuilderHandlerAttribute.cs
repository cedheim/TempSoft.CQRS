using System;

namespace TempSoft.CQRS.Queries
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class QueryBuilderHandlerAttribute : Attribute
    {
        public Type For { get; }

        public QueryBuilderHandlerAttribute(Type @for)
        {
            For = @for;
        }
    }
}