using System;
using System.Runtime.Serialization;
using TempSoft.CQRS.Exceptions;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Exceptions
{
    [Serializable]
    public class NotInitializedException : DomainException
    {
        public NotInitializedException()
        {
        }

        public NotInitializedException(Guid id) : base($"Tried to manipulate and aggregate root ({id}) which was not initialized.")
        {
        }
        
        protected NotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}