using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Infrastructure;

namespace TempSoft.CQRS.Domain
{
    public class AggregateRootRepository : IAggregateRootRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;
        private readonly ICommandRegistry _commandRegistry;

        public AggregateRootRepository(IEventStore eventStore, IEventBus eventBus, ICommandRegistry commandRegistry)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _commandRegistry = commandRegistry;
        }

        public async Task<IAggregateRoot> Get(Type type, Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var getEventsTask = _eventStore.Get(id, cancellationToken: cancellationToken);
            var getCommandIdsTask = _commandRegistry.Get(id, cancellationToken);

            var events = (await getEventsTask)?.ToArray();
            var commandIds = (await getCommandIdsTask)?.ToArray();

            var root = Activate(type);
            if (!object.ReferenceEquals(events, default(IEnumerable<IEvent>)) && events.Length > 0)
            {
                root.LoadFrom(events, commandIds ?? Enumerable.Empty<Guid>());
            }

            return root;
        }

        public async Task<TAggregate> Get<TAggregate>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot
        {
            return (TAggregate)(await this.Get(typeof(TAggregate), id, cancellationToken));
        }

        public async Task Save(IAggregateRoot root, CancellationToken cancellationToken = default(CancellationToken))
        {
            var commit = root.Commit();

            var saveEventsTask = _eventStore.Save(commit.Events, cancellationToken);
            var saveCommandIdsTask = _commandRegistry.Save(commit.AggregateRootId, commit.CommandIds, cancellationToken);

            await Task.WhenAll(saveCommandIdsTask, saveEventsTask);
            await _eventBus.Publish(commit.Events, cancellationToken);
        }

        private IAggregateRoot Activate(Type type)
        {
            return (IAggregateRoot) Services.Resolve(type);
        }
    }
}