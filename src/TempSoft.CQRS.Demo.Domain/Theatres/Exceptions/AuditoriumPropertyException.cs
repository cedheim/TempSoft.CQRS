using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Exceptions
{
    [Serializable]
    public class AuditoriumPropertyException : Exception
    {
        public AuditoriumPropertyException()
        {
        }

        public AuditoriumPropertyException(string message) : base(message)
        {
        }

        public AuditoriumPropertyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AuditoriumPropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}