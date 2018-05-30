using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Events
{
    public interface IEventBusService : IService
    {
        Task Publish(EventMessage[] messages, CancellationToken cancellationToken);
    }
}