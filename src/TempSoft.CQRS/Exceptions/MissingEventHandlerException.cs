using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class MissingEventHandlerException : DomainException
    {
        public MissingEventHandlerException()
        {
        }

        public MissingEventHandlerException(string message) : base(message)
        {
        }

        public MissingEventHandlerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingEventHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}