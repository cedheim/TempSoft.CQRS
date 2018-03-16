using System;
using System.Runtime.Serialization;

namespace NCG.NGS.CQRS.Exception
{
    [Serializable]
    public class EventsOutOfOrderException : DomainException
    {
        public EventsOutOfOrderException()
        {
        }

        public EventsOutOfOrderException(string message) : base(message)
        {
        }

        public EventsOutOfOrderException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected EventsOutOfOrderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}