using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exception
{
    [Serializable]
    public class MissingQueryBuilderHandlerException : System.Exception
    {
        public MissingQueryBuilderHandlerException()
        {
        }

        public MissingQueryBuilderHandlerException(string message) : base(message)
        {
        }

        public MissingQueryBuilderHandlerException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        protected MissingQueryBuilderHandlerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}