using System;
using System.Runtime.Serialization;
using TempSoft.CQRS.Exceptions;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Exceptions
{
    [Serializable]
    public class DoubleInitializationException : DomainException
    {
        public DoubleInitializationException()
        {
        }

        public DoubleInitializationException(Guid id) : base($"Tried to double initialize aggregate root ({id})")
        {
        }

        protected DoubleInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}