using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.ServiceFabric.Exceptions
{
    [Serializable]
    public class AggregateRootHasWrongIdException : Exception
    {
        public AggregateRootHasWrongIdException()
        {
        }

        public AggregateRootHasWrongIdException(string message) : base(message)
        {
        }

        public AggregateRootHasWrongIdException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        protected AggregateRootHasWrongIdException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}