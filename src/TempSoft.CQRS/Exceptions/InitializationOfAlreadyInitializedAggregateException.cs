using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class InitializationOfAlreadyInitializedAggregateException : DomainException
    {
        public InitializationOfAlreadyInitializedAggregateException()
        {
        }

        public InitializationOfAlreadyInitializedAggregateException(string message) : base(message)
        {
        }

        public InitializationOfAlreadyInitializedAggregateException(string message, Exception innerException) : base(
            message, innerException)
        {
        }

        protected InitializationOfAlreadyInitializedAggregateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}