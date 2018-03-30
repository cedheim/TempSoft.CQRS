﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TempSoft.CQRS.Common.Extensions;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract]
    public class GetReadModelMessage : MessageBase
    {
        [DataMember(Name = "AggregateRootType")]
        private string _aggregateRootType;

        [IgnoreDataMember]
        private Type _deserializedAggregateRootType;

        private GetReadModelMessage()
        {
        }

        public GetReadModelMessage(Type aggregateRootType, IEnumerable<KeyValuePair<string, object>> headers = null)
            : base(headers)
        {
            _aggregateRootType = aggregateRootType.ToFriendlyName();
        }


        [IgnoreDataMember]
        public Type AggregateRootType => _deserializedAggregateRootType ?? (_deserializedAggregateRootType = Type.GetType(_aggregateRootType));

    }
}