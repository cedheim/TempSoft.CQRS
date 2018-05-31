using System;
using System.Runtime.Serialization;
using TempSoft.CQRS.Exceptions;

namespace TempSoft.CQRS.ServiceFabric.Exceptions
{
    [Serializable]
    public class ProjectorTypeMissmatchException : DomainException
    {
        public ProjectorTypeMissmatchException()
        {
        }

        public ProjectorTypeMissmatchException(string message) : base(message)
        {
        }

        public ProjectorTypeMissmatchException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ProjectorTypeMissmatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}