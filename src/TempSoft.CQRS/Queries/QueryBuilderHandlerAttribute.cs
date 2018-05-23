using System;

namespace TempSoft.CQRS.Queries
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class QueryBuilderHandlerAttribute : Attribute
    {
        public QueryBuilderHandlerAttribute(Type @for)
        {
            For = @for;
        }

        public Type For { get; }
    }
}