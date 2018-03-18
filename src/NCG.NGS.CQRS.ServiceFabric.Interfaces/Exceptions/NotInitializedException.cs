using System;
using System.Runtime.Serialization;

namespace NCG.NGS.CQRS.ServiceFabric.Interfaces.Exceptions
{
    [Serializable]
    public class NotInitializedException : System.Exception
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