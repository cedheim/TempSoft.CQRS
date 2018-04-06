using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Demo.Domain.Theatres.Exceptions
{
    [Serializable]
    public class DuplicateAuditoriumAddedException : Exception
    {
        public DuplicateAuditoriumAddedException()
        {
        }

        public DuplicateAuditoriumAddedException(string message) : base(message)
        {
        }

        public DuplicateAuditoriumAddedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DuplicateAuditoriumAddedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}