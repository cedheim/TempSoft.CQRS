using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class ProjectionIdentifierException : InfrastructureException
    {
        public ProjectionIdentifierException()
        {
        }

        public ProjectionIdentifierException(string message) : base(message)
        {
        }

        public ProjectionIdentifierException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProjectionIdentifierException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}