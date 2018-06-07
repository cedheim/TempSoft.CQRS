using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class MissingProjectorException : DomainException
    {
        public MissingProjectorException()
        {
        }

        public MissingProjectorException(string message) : base(message)
        {
        }

        public MissingProjectorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingProjectorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}