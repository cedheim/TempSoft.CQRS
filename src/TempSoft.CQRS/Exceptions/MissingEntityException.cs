using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class MissingEntityException : DomainException
    {
        public MissingEntityException()
        {
        }

        public MissingEntityException(string message) : base(message)
        {
        }

        public MissingEntityException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MissingEntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}