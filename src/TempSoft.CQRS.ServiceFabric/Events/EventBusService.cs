using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Runtime;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Events
{
    public class EventBusService : StatefulService, IEventBusService
    {
        public EventBusService(StatefulServiceContext serviceContext) : base(serviceContext)
        {
        }

        public EventBusService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica) : base(serviceContext, reliableStateManagerReplica)
        {
        }

        public Task Publish(EventMessage[] messages, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}