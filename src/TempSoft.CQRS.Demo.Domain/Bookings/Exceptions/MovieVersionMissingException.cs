using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Demo.Domain.Bookings.Exceptions
{
    [Serializable]
    public class MovieVersionMissingException : Exception
    {
        public MovieVersionMissingException()
        {
        }

        public MovieVersionMissingException(string message) : base(message)
        {
        }

        public MovieVersionMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MovieVersionMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}