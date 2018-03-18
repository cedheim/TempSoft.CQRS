using System.Runtime.Serialization;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Events
{
    [DataContract]
    public class EventState : GenericObjectBase
    {
        private EventState()
        {
        }

        public EventState(IEvent @event)
            : base(@event)
        {
        }

        public new IEvent Body => (IEvent) base.Body;

        public T GetEvent<T>() where T : IEvent
        {
            return (T) Body;
        }
    }
}