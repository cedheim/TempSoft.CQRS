﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Extensions;
using TempSoft.CQRS.Infrastructure;

namespace TempSoft.CQRS.Domain
{
    public class AggregateRootRepository : IAggregateRootRepository
    {
        private readonly ICommandRegistry _commandRegistry;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;
        private readonly IEventStore _eventStore;

        public AggregateRootRepository(IEventStore eventStore, IEventBus eventBus, ICommandRegistry commandRegistry, IServiceProvider serviceProvider)
        {
            _eventStore = eventStore;
            _eventBus = eventBus;
            _commandRegistry = commandRegistry;
            _serviceProvider = serviceProvider;
        }

        public async Task<IAggregateRoot> Get(Type type, string id, bool createIfItDoesNotExist = true, CancellationToken cancellationToken = default(CancellationToken))
        {
            var getEventsTask = _eventStore.Get(id, cancellationToken: cancellationToken);
            var getCommandIdsTask = _commandRegistry.Get(id, cancellationToken);

            var events = (await getEventsTask)?.ToArray();
            var commandIds = (await getCommandIdsTask)?.ToArray();

            if ((object.ReferenceEquals(events, default(IEvent[])) || events.Length == 0) && !createIfItDoesNotExist)
            {
                return default(IAggregateRoot);
            }

            var root = Activate(type);
            root.Id = id;

            if (!ReferenceEquals(events, default(IEnumerable<IEvent>)) && events.Length > 0)
                root.LoadFrom(events, commandIds ?? Enumerable.Empty<Guid>());

            return root;
        }

        public async Task<TAggregate> Get<TAggregate>(string id, bool createIfItDoesNotExist = true, CancellationToken cancellationToken = default(CancellationToken)) 
            where TAggregate : IAggregateRoot
        {
            return (TAggregate)await Get(typeof(TAggregate), id, createIfItDoesNotExist, cancellationToken);
        }

        public async Task Save(IAggregateRoot root, CancellationToken cancellationToken = default(CancellationToken))
        {
            var commit = root.Commit();

            var saveEventsTask = _eventStore.Save(commit.Events, cancellationToken);
            var saveCommandIdsTask =
                _commandRegistry.Save(commit.AggregateRootId, commit.CommandIds, cancellationToken);

            await Task.WhenAll(saveCommandIdsTask, saveEventsTask);
            await _eventBus.Publish(commit.Events, cancellationToken);
        }

        private IAggregateRoot Activate(Type type)
        {
            return (IAggregateRoot) _serviceProvider.GetService(type);
        }
    }
}