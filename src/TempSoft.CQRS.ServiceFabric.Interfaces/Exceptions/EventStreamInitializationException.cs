using System;
using System.Runtime.Serialization;
using TempSoft.CQRS.Exceptions;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Exceptions
{
    [Serializable]
    public class EventStreamInitializationException : InfrastructureException
    {
        public EventStreamInitializationException()
        {
        }

        public EventStreamInitializationException(string message) : base(message)
        {
        }

        public EventStreamInitializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EventStreamInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}