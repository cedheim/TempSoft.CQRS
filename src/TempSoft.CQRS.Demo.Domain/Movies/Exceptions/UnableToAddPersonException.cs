using System;
using System.Runtime.Serialization;
using TempSoft.CQRS.Exceptions;

namespace TempSoft.CQRS.Demo.Domain.Movies.Exceptions
{
    [Serializable]
    public class UnableToAddPersonException : DomainException
    {
        public UnableToAddPersonException()
        {
        }

        public UnableToAddPersonException(string message) : base(message)
        {
        }

        public UnableToAddPersonException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UnableToAddPersonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}