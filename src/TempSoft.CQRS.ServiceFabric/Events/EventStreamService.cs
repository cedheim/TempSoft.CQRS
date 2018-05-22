using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Exceptions;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.ServiceFabric.Events
{
    public class EventStreamService : StatefulService, IEventStreamService
    {
        public const string EventStreamQueue = "_tempsoft_cqrs_event_queue";

        private readonly IEventStreamStateManager _streamStateManager;
        private readonly IFabricClient _fabricClient;
        private readonly IEventStore _eventStore;
        private readonly IEventStreamRegistry _eventStreamRegistry;
        
        private string _streamName;
        private EventStreamDefinition _definition;

        public EventStreamService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica, IEventStreamStateManager streamStateManager, IFabricClient fabricClient, IEventStore eventStore, IEventStreamRegistry eventStreamRegistry) : base(serviceContext, reliableStateManagerReplica)
        {
            _streamStateManager = streamStateManager;
            _fabricClient = fabricClient;
            _eventStore = eventStore;
            _eventStreamRegistry = eventStreamRegistry;

        }

        public EventStreamService(StatefulServiceContext serviceContext, IEventStreamStateManager streamStateManager, IFabricClient fabricClient, IEventStore eventStore, IEventStreamRegistry eventStreamRegistry) : base(serviceContext)
        {
            _streamStateManager = streamStateManager;
            _fabricClient = fabricClient;
            _eventStore = eventStore;
            _eventStreamRegistry = eventStreamRegistry;
        }

        protected override async Task OnOpenAsync(ReplicaOpenMode openMode, CancellationToken cancellationToken)
        {
            await base.OnOpenAsync(openMode, cancellationToken);

            var partitions = await _fabricClient.QueryManager.GetPartitionAsync(Context.PartitionId);
            var namedPartition = (NamedPartitionInformation)(partitions.FirstOrDefault(p => p.PartitionInformation.Id == Context.PartitionId)?.PartitionInformation);
            if (namedPartition == null)
            {
                throw new EventStreamInitializationException($"Unable to find name for partition {Context.PartitionId}");
            }

            _streamName = namedPartition.Name;
            _definition = _eventStreamRegistry.GetEventStreamByName(_streamName);

            if (_definition == null)
            {
                throw new EventStreamInitializationException($"Unable to find stream definition for {_streamName}");
            }
        }


        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();


            var status = await _streamStateManager.GetStatusForStream(_streamName);


            if (status != EventStreamStatus.Initialized)
            {
                await Initialize(cancellationToken);
            }

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }

        private async Task Initialize(CancellationToken cancellationToken)
        {
            await _streamStateManager.SetStatusForStream(_streamName, EventStreamStatus.Initializing);
            
            var eventFilter = new EventStoreFilter() { EventGroups = _definition.Filter.EventGroups?.ToArray(), EventTypes = _definition.Filter.EventTypes?.Select(t => t.ToFriendlyName())?.ToArray() };

            await _eventStore.List(eventFilter, async (@event, token) =>
            {
                await AddEventToStream(new EventMessage(@event), token);
            }, cancellationToken);

            await _streamStateManager.SetStatusForStream(_streamName, EventStreamStatus.Initialized);
        }

        private async Task AddEventToStream(EventMessage message, CancellationToken cancellationToken)
        {
            var queue = await StateManager.GetOrAddAsync<IReliableQueue<EventMessage>>(EventStreamQueue);

            while (true)
            {
                try
                {
                    using (var transaction = this.StateManager.CreateTransaction())
                    {
                        await queue.EnqueueAsync(transaction, message);
                        await transaction.CommitAsync();
                    }

                    break;
                }
                catch (TimeoutException)
                {
                    await Task.Delay(100, cancellationToken);
                }
            }

            await _streamStateManager.AddToEventCountForStream(_streamName);
        }

        public async Task Write(EventMessage message, CancellationToken cancellationToken)
        {
            await AddEventToStream(message, cancellationToken);
        }

        public async Task<EventMessage> Read(TimeSpan timeout, CancellationToken cancellationToken)
        {
            var start = DateTime.UtcNow;
            var end = start + timeout;

            var queue = await StateManager.GetOrAddAsync<IReliableQueue<EventMessage>>(EventStreamQueue);

            while (end > DateTime.UtcNow)
            {
                try
                {
                    using (var transaction = this.StateManager.CreateTransaction())
                    {
                        var conditionalResult = await queue.TryDequeueAsync(transaction, TimeSpan.FromMilliseconds(100), cancellationToken);
                        await transaction.CommitAsync();

                        if (conditionalResult.HasValue)
                        {
                            return conditionalResult.Value;
                        }
                    }

                }
                catch (TimeoutException) { }

                await Task.Delay(100, cancellationToken);
            }

            return default(EventMessage);
        }
    }
}