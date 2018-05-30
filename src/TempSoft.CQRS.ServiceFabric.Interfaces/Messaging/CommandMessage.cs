using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Common.Extensions;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract(Namespace = ContractConstants.Namespace)]
    public class CommandMessage : GenericMessage
    {
        [DataMember(Name = "AggregateRootType")]
        private string _aggregateRootType;

        [IgnoreDataMember] private Type _deserializedAggregateRootType;

        public CommandMessage(Type aggregateRootType, ICommand command,
            IEnumerable<KeyValuePair<string, object>> headers = null) : base(command, headers)
        {
            _aggregateRootType = aggregateRootType.ToFriendlyName();
        }

        private CommandMessage()
        {
        }

        [IgnoreDataMember]
        public Type AggregateRootType => _deserializedAggregateRootType ??
                                         (_deserializedAggregateRootType = Type.GetType(_aggregateRootType));

        [IgnoreDataMember]
        public new ICommand Body => (ICommand) base.Body;

        public TCommand GetCommand<TCommand>()
        {
            return (TCommand) Body;
        }
    }
}