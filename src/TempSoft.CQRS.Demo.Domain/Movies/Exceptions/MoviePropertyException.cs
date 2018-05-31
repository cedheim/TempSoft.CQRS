using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Demo.Domain.Movies.Exceptions
{
    [Serializable]
    public class MoviePropertyException : Exception
    {
        public MoviePropertyException()
        {
        }

        public MoviePropertyException(string message) : base(message)
        {
        }

        public MoviePropertyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MoviePropertyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}