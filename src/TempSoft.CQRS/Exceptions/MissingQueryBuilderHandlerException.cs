using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class MissingQueryBuilderHandlerException : Exception
    {
        public MissingQueryBuilderHandlerException()
        {
        }

        public MissingQueryBuilderHandlerException(string message) : base(message)
        {
        }

        public MissingQueryBuilderHandlerException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        protected MissingQueryBuilderHandlerException(SerializationInfo info, StreamingContext context) : base(info,
            context)
        {
        }
    }
}