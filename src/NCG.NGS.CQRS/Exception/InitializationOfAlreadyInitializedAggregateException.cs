using System;
using System.Runtime.Serialization;

namespace NCG.NGS.CQRS.Exception
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

        public InitializationOfAlreadyInitializedAggregateException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected InitializationOfAlreadyInitializedAggregateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}