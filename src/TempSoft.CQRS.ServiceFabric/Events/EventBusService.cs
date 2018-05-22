using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.Common.Random;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Queries;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Interfaces.Queries;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.ServiceFabric.Events
{
    public class EventBusService : StatefulService, IEventBusService
    {
        private readonly IEventStreamRegistry _eventStreamRegistry;
        private readonly IApplicationUriBuilder _uriBuilder;
        private readonly IServiceProxyFactory _serviceProxyFactory;
        private readonly IActorProxyFactory _actorProxyFactory;

        private const string EventQueueName = "_tempsoft_event_queue";

        public EventBusService(StatefulServiceContext serviceContext, IEventStreamRegistry eventStreamRegistry, IApplicationUriBuilder uriBuilder, IServiceProxyFactory serviceProxyFactory, IActorProxyFactory actorProxyFactory) : base(serviceContext)
        {
            _eventStreamRegistry = eventStreamRegistry;
            _uriBuilder = uriBuilder;
            _serviceProxyFactory = serviceProxyFactory;
            _actorProxyFactory = actorProxyFactory;
        }

        public EventBusService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica, IEventStreamRegistry eventStreamRegistry, IApplicationUriBuilder uriBuilder, IServiceProxyFactory serviceProxyFactory, IActorProxyFactory actorProxyFactory) : base(serviceContext, reliableStateManagerReplica)
        {
            _eventStreamRegistry = eventStreamRegistry;
            _uriBuilder = uriBuilder;
            _serviceProxyFactory = serviceProxyFactory;
            _actorProxyFactory = actorProxyFactory;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var queue = await StateManager.GetOrAddAsync<IReliableQueue<EventMessage>>(EventQueueName);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var tx = StateManager.CreateTransaction())
                    {
                        var conditionalEvent = await queue.TryDequeueAsync(tx, TimeSpan.FromMilliseconds(100), cancellationToken);
                        if (!conditionalEvent.HasValue)
                        {
                            await Task.Delay(100, cancellationToken);
                            continue;
                        }

                        var eventMessage = conditionalEvent.Value;

                        var definitions = _eventStreamRegistry.GetEventStreamsByEvent(eventMessage.Body);
                        var writeTasks = definitions.Select(definition => Task.Run(async () =>
                        {
                            var uri = _uriBuilder.Build(nameof(EventStreamService)).ToUri();
                            var proxy = _serviceProxyFactory.CreateServiceProxy<IEventStreamService>(uri, new ServicePartitionKey(definition.Name));
                            await proxy.Write(eventMessage, cancellationToken);
                        }, cancellationToken));

                        await Task.WhenAll(writeTasks);

                        await tx.CommitAsync();
                    }
                }
                catch (OperationCanceledException e)
                {
                    throw;
                }
                catch (FabricException e)
                {
                    throw;
                }
                catch (Exception e)
                {

                }
            }
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }
        
        public async Task Publish(EventMessage[] events, CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var queue = await StateManager.GetOrAddAsync<IReliableQueue<EventMessage>>(EventQueueName);

            while (true)
            {
                try
                {
                    using (var transaction = this.StateManager.CreateTransaction())
                    {
                        foreach (var eventMessage in events)
                        {
                            await queue.EnqueueAsync(transaction, eventMessage, TimeSpan.FromMilliseconds(100), cancellationToken);
                        }

                        await transaction.CommitAsync();
                    }

                    break;
                }
                catch (TimeoutException)
                {
                    await Task.Delay(100, CancellationToken.None);
                }
            }


            stopwatch.Stop();
        }
    }
}