﻿using System;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Exceptions
{
    public class DoubleInitializationException : System.Exception
    {
        public DoubleInitializationException()
        {
        }

        public DoubleInitializationException(Guid id) : base($"Tried to double initialize aggregate root ({id})")
        {
        }
        
        protected DoubleInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}