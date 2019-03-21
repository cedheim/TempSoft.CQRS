using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Extensions;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract(Namespace = ContractConstants.Namespace)]
    public class ProjectorMessage : GenericMessage
    {
        [DataMember(Name = "ProjectorType")]
        private string _projectorType;

        [IgnoreDataMember] private Type _deserializedProjectorType;

        private ProjectorMessage()
        {
        }

        public ProjectorMessage(IEvent value, Type projectorType, IEnumerable<KeyValuePair<string, object>> headers = null)
            : base(value, headers)
        {
            _projectorType = projectorType.ToFriendlyName();
            _deserializedProjectorType = projectorType;
        }
        
        [IgnoreDataMember]
        public Type ProjectorType => _deserializedProjectorType ?? (_deserializedProjectorType = Type.GetType(_projectorType));

        [IgnoreDataMember]
        public new IEvent Body => (IEvent)base.Body;

        public TEvent GetEvent<TEvent>() where TEvent : IEvent
        {
            return (TEvent)Body;
        }
    }
}