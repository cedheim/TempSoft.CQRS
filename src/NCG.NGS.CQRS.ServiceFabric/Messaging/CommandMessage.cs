using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using NCG.NGS.CQRS.Commands;
using NCG.NGS.CQRS.ServiceFabric.Extensions;

namespace NCG.NGS.CQRS.ServiceFabric.Messaging
{
    [DataContract]
    public class CommandMessage : GenericMessage
    {
        [DataMember(Name = "AggregateRootType")]
        private string _aggregateRootType;

        [IgnoreDataMember]
        private Type _deserializedAggregateRootType;

        public CommandMessage(Type aggregateRootType, ICommand command, IEnumerable<KeyValuePair<string, object>> headers = null) : base(command, headers)
        {
            _aggregateRootType = aggregateRootType.ToFriendlyName();
        }

        private CommandMessage()
        {
        }
        
        [IgnoreDataMember]
        public Type AggregateRootType => _deserializedAggregateRootType ?? (_deserializedAggregateRootType = Type.GetType(_aggregateRootType));

        public new ICommand Body => (ICommand)base.Body;

        public TCommand GetCommand<TCommand>() => (TCommand) Body;
    }
}