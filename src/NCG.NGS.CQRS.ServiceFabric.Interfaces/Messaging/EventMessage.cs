using System.Collections.Generic;
using System.Runtime.Serialization;
using NCG.NGS.CQRS.Events;

namespace NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging
{
    [DataContract]
    public class EventMessage : GenericMessage
    {
        public EventMessage(IEvent value, IEnumerable<KeyValuePair<string, object>> headers = null) : base(value, headers)
        {
        }

        private EventMessage()
        {
        }

        [IgnoreDataMember] public new IEvent Body => (IEvent)base.Body;

        public TEvent GetEvent<TEvent>() where TEvent : IEvent
        {
            return (TEvent) Body;
        }
    }
}