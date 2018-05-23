using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Interfaces.Events
{
    public interface IEventStreamService : IService
    {
        Task Write(EventMessage message, CancellationToken cancellationToken);

        Task<EventMessage> Read(TimeSpan timeout, CancellationToken cancellationToken);
    }
}