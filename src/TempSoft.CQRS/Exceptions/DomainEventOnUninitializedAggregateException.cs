using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    public class DomainEventOnUninitializedAggregateException : DomainException
    {
        public DomainEventOnUninitializedAggregateException()
        {
        }

        public DomainEventOnUninitializedAggregateException(string message) : base(message)
        {
        }

        public DomainEventOnUninitializedAggregateException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        protected DomainEventOnUninitializedAggregateException(SerializationInfo info, StreamingContext context) : base(
            info, context)
        {
        }
    }
}