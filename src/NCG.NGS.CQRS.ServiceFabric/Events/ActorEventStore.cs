using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;
using NCG.NGS.CQRS.Events;

namespace NCG.NGS.CQRS.ServiceFabric.Events
{
    public class ActorEventStore : IEventStore
    {
        private const string EventStreamStateName = "_ncg_ngs_cqrs_event_stream";

        private readonly IActorStateManager _stateManager;

        public ActorEventStore(IActorStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public async Task<IEnumerable<IEvent>> Get(Guid id, int fromVersion = default(int), CancellationToken cancellationToken = default(CancellationToken))
        {
            var tryGetEventStream = await _stateManager.TryGetStateAsync<EventStream>(EventStreamStateName, cancellationToken);

            return tryGetEventStream.HasValue 
                ? tryGetEventStream.Value
                    .Where(e => e.Version > fromVersion)
                    .OrderBy(e => e.Version) 
                : Enumerable.Empty<IEvent>();
        }

        public async Task Save(Guid id, IEnumerable<IEvent> events, CancellationToken cancellationToken = default(CancellationToken))
        {
            var eventStream = new EventStream(events);

            await _stateManager.AddOrUpdateStateAsync(EventStreamStateName, eventStream, (s, stream) => stream.AddToEnd(eventStream), cancellationToken);
        }
    }
}