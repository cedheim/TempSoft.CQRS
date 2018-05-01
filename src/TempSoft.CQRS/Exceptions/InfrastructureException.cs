﻿using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class InfrastructureException : Exception
    {
        public InfrastructureException()
        {
        }

        public InfrastructureException(string message) : base(message)
        {
        }

        public InfrastructureException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InfrastructureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}