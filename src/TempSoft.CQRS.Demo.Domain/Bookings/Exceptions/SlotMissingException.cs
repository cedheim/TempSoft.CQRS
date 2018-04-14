using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Demo.Domain.Bookings.Exceptions
{
    [Serializable]
    public class SlotMissingException : Exception
    {
        public SlotMissingException()
        {
        }

        public SlotMissingException(string message) : base(message)
        {
        }

        public SlotMissingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SlotMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}