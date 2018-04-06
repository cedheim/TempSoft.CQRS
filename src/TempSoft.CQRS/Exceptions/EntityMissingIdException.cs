using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class EntityMissingIdException : DomainException
    {
        public EntityMissingIdException()
        {
        }

        public EntityMissingIdException(string message) : base(message)
        {
        }

        public EntityMissingIdException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EntityMissingIdException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}