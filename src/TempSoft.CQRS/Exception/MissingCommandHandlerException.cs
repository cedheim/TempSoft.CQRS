using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exception
{
    [Serializable]
    public class MissingCommandHandlerException : DomainException
    {
        public MissingCommandHandlerException()
        {
        }

        public MissingCommandHandlerException(string message) : base(message)
        {
        }

        public MissingCommandHandlerException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected MissingCommandHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}