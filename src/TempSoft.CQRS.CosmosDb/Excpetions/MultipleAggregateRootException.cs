using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.CosmosDb.Excpetions
{
    [Serializable]
    public class MultipleAggregateRootException : Exception
    {
        public MultipleAggregateRootException()
        {
        }

        public MultipleAggregateRootException(string message) : base(message)
        {
        }

        public MultipleAggregateRootException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MultipleAggregateRootException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}