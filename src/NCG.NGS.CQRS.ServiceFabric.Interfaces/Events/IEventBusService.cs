using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Interfaces.Events
{
    public interface IEventBusService : IService
    {
        Task Publish(EventMessage[] events, CancellationToken cancellationToken);
    }
}