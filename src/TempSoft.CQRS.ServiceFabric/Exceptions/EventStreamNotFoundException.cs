using System;
using System.Runtime.Serialization;
using TempSoft.CQRS.Exceptions;

namespace TempSoft.CQRS.ServiceFabric.Exceptions
{
    [Serializable]
    public class EventStreamNotFoundException : InfrastructureException
    {
        public EventStreamNotFoundException()
        {
        }

        public EventStreamNotFoundException(string message) : base(message)
        {
        }

        public EventStreamNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EventStreamNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}