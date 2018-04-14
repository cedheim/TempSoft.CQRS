using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Demo.Domain.Bookings.Exceptions
{
    [Serializable]
    public class AuditoriumMissingException : Exception
    {
        public AuditoriumMissingException()
        {
        }

        public AuditoriumMissingException(string message) : base(message)
        {
        }

        public AuditoriumMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AuditoriumMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}