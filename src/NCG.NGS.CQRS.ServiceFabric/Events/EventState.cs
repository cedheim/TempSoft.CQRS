using System.Runtime.Serialization;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Events
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