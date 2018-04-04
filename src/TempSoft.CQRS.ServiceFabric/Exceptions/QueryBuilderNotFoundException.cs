using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.ServiceFabric.Exceptions
{
    [Serializable]
    public class QueryBuilderNotFoundException : System.Exception
    {
        public QueryBuilderNotFoundException()
        {
        }

        public QueryBuilderNotFoundException(string message) : base(message)
        {
        }

        public QueryBuilderNotFoundException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected QueryBuilderNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}