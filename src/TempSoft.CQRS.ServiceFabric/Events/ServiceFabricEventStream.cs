using System;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;

namespace TempSoft.CQRS.ServiceFabric.Events
{
    public class ServiceFabricEventStream : IEventStream
    {
        private readonly IEventStreamService _service;
        private readonly EventStreamDefinition _definition;

        public ServiceFabricEventStream(EventStreamDefinition definition,IEventStreamService service)
        {
            _service = service;
            _definition = definition;
        }

        public async Task<IEvent> Read(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var message = await _service.Read(timeout, cancellationToken);
            return message?.Body;
        }
    }
}