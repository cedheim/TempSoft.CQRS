using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TempSoft.CQRS.Exceptions
{
    [Serializable]
    public class BootstrapperValidationException : InfrastructureException
    {
        public BootstrapperValidationException()
        {
        }

        public BootstrapperValidationException(Type[] missingServices) : base($"Unable to resolve the following required services: {missingServices.Aggregate(string.Empty, (s, failure) => s + ", " + failure.Name)}")
        {
            MissingServices = missingServices;
        }

        public BootstrapperValidationException(Type[] missingServices, Exception innerException) : base($"Unable to resolve the following required services: {missingServices.Aggregate(string.Empty, (s, failure) => s + ", " + failure.Name)}", innerException)
        {
            MissingServices = missingServices;
        }

        protected BootstrapperValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public Type[] MissingServices { get; }
    }
}