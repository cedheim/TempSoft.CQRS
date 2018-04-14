using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Exceptions
{
    [Serializable]
    public class DuplicateSlotException : Exception
    {
        public DuplicateSlotException()
        {
        }

        public DuplicateSlotException(string message) : base(message)
        {
        }

        public DuplicateSlotException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateSlotException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}