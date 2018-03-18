using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NCG.NGS.CQRS.Common.Extensions;

namespace NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract]
    public class InitializeMessage : GenericMessage
    {
        [IgnoreDataMember] private Type _aggregateRootType;

        public InitializeMessage(Type aggregateRootType, IEnumerable<KeyValuePair<string, object>> headers = null)
            : base(aggregateRootType.ToFriendlyName(), headers)
        {
            _aggregateRootType = aggregateRootType;
        }

        private InitializeMessage()
        {
        }

        [IgnoreDataMember]
        public Type AggregateRootType => _aggregateRootType ?? (_aggregateRootType = Type.GetType((string) Body));

    }
}