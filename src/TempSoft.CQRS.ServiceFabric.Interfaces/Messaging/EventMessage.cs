using System.Collections.Generic;
using System.Runtime.Serialization;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract(Namespace = ContractConstants.Namespace)]
    public class EventMessage : GenericMessage
    {
        public EventMessage(IEvent value, IEnumerable<KeyValuePair<string, object>> headers = null) : base(value,
            headers)
        {
        }

        private EventMessage()
        {
        }

        [IgnoreDataMember] public new IEvent Body => (IEvent) base.Body;

        public TEvent GetEvent<TEvent>() where TEvent : IEvent
        {
            return (TEvent) Body;
        }
    }
}