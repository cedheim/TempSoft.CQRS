using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class InitializationEventMissingAggregateRootIdException : DomainException
    {
        public InitializationEventMissingAggregateRootIdException()
        {
        }

        public InitializationEventMissingAggregateRootIdException(string message) : base(message)
        {
        }

        public InitializationEventMissingAggregateRootIdException(string message, Exception innerException) : base(
            message, innerException)
        {
        }

        protected InitializationEventMissingAggregateRootIdException(SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
        }
    }
}