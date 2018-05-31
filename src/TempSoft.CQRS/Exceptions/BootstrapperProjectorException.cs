using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class BootstrapperProjectorException : InfrastructureException
    {
        public BootstrapperProjectorException()
        {
        }

        public BootstrapperProjectorException(string message) : base(message)
        {
        }

        public BootstrapperProjectorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BootstrapperProjectorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}