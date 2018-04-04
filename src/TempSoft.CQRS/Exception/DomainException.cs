using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exception
{
    [Serializable]
    public class DomainException : System.Exception
    {
        public DomainException()
        {
        }

        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected DomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}