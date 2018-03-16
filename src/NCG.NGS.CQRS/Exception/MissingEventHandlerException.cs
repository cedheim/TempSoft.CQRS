using System;
using System.Runtime.Serialization;

namespace NCG.NGS.CQRS.Exception
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

        public MissingEventHandlerException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected MissingEventHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}