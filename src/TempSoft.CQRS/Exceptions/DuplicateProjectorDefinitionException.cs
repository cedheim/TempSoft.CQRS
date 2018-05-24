using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class DuplicateProjectorDefinitionException : InfrastructureException
    {
        public DuplicateProjectorDefinitionException()
        {
        }

        public DuplicateProjectorDefinitionException(string message) : base(message)
        {
        }

        public DuplicateProjectorDefinitionException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        protected DuplicateProjectorDefinitionException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}