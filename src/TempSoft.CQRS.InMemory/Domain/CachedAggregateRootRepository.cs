using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.InMemory.Domain
{
    public class CachedAggregateRootRepository : IAggregateRootRepository
    {
        private readonly ConcurrentDictionary<Guid, IAggregateRoot> _cache = new ConcurrentDictionary<Guid, IAggregateRoot>();
        private readonly IAggregateRootRepository _internalRepository;

        public CachedAggregateRootRepository(IEventStore eventStore, IEventBus eventBus, ICommandRegistry commandRegistry, IServiceProvider serviceProvider)
        {
            _internalRepository = new AggregateRootRepository(eventStore, eventBus, commandRegistry, serviceProvider);
        }

        public Task<IAggregateRoot> Get(Type type, Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var root = _cache.GetOrAdd(id, guid => _internalRepository.Get(type, guid, cancellationToken).GetAwaiter().GetResult());
            return Task.FromResult(root);
        }

        public Task<TAggregate> Get<TAggregate>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot
        {
            var root = _cache.GetOrAdd(id, guid => _internalRepository.Get(typeof(TAggregate), guid, cancellationToken).GetAwaiter().GetResult());
            return Task.FromResult((TAggregate)root);
        }

        public async Task Save(IAggregateRoot root, CancellationToken cancellationToken = default(CancellationToken))
        {
            _cache.AddOrUpdate(root.Id, root, (guid, aggregateRoot) => root);
            await _internalRepository.Save(root, cancellationToken);
        }
    }
}