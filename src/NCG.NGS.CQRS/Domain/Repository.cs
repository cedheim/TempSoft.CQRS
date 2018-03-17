using System;
using System.Threading;
using System.Threading.Tasks;
using NCG.NGS.CQRS.Commands;
using NCG.NGS.CQRS.Events;

namespace NCG.NGS.CQRS.Domain
{
    public class Repository : IRepository
    {
        private readonly IEventStore _eventStore;
        private readonly IEventBus _eventBus;
        private readonly ICommandRegistry _commandRegistry;

        public Repository(IEventStore eventStore, IEventBus eventBus, ICommandRegistry commandRegistry)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _commandRegistry = commandRegistry;
        }

        public Task<IAggregateRoot> Get(Type type, Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<TAggregate> Get<TAggregate>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot
        {
            throw new NotImplementedException();
        }

        public Task Save(IAggregateRoot root, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}