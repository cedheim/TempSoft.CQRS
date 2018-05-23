using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class DuplicateEventStreamDefinitionException : InfrastructureException
    {
        public DuplicateEventStreamDefinitionException()
        {
        }

        public DuplicateEventStreamDefinitionException(string message) : base(message)
        {
        }

        public DuplicateEventStreamDefinitionException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        protected DuplicateEventStreamDefinitionException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}