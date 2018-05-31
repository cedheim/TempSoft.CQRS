using System;
using System.Collections.Generic;
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
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Interfaces.Projectors;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.ServiceFabric.Events
{
    public class EventBusService : StatefulService, IEventBusService
    {
        private const string EventQueueName = "_tempsoft_event_queue";

        private readonly IProjectorRegistry _projectorRegistry;
        private readonly IUriHelper _uriHelper;
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly IServiceProxyFactory _serviceProxyFactory;
        private readonly Lazy<IReliableQueue<EventMessage>> _lazyQueue;
        private readonly Uri _projectorActorUri;

        public EventBusService(StatefulServiceContext serviceContext, IProjectorRegistry projectorRegistry, IUriHelper uriHelper, IActorProxyFactory actorProxyFactory, IServiceProxyFactory serviceProxyFactory) : base(serviceContext)
        {
            _lazyQueue = new Lazy<IReliableQueue<EventMessage>>(() => this.StateManager.GetOrAddAsync<IReliableQueue<EventMessage>>(EventQueueName).GetAwaiter().GetResult());
            _projectorRegistry = projectorRegistry;
            _uriHelper = uriHelper;
            _actorProxyFactory = actorProxyFactory;
            _serviceProxyFactory = serviceProxyFactory;
            _projectorActorUri = uriHelper.GetUriFor<IProjectorActor>();
        }

        public EventBusService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica, IProjectorRegistry projectorRegistry, IUriHelper uriHelper, IActorProxyFactory actorProxyFactory, IServiceProxyFactory serviceProxyFactory) : base(serviceContext, reliableStateManagerReplica)
        {
            _lazyQueue = new Lazy<IReliableQueue<EventMessage>>(() => this.StateManager.GetOrAddAsync<IReliableQueue<EventMessage>>(EventQueueName).GetAwaiter().GetResult());
            _projectorRegistry = projectorRegistry;
            _uriHelper = uriHelper;
            _actorProxyFactory = actorProxyFactory;
            _serviceProxyFactory = serviceProxyFactory;
            _projectorActorUri = uriHelper.GetUriFor<IProjectorActor>();
        }

        public IReliableQueue<EventMessage> Queue => _lazyQueue.Value;

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var tx = StateManager.CreateTransaction())
                    {
                        var conditionalEvent = await Queue.TryDequeueAsync(tx, TimeSpan.FromMilliseconds(100), cancellationToken);
                        if (!conditionalEvent.HasValue)
                        {
                            await Task.Delay(100, cancellationToken);
                            continue;
                        }

                        var eventMessage = conditionalEvent.Value;

                        var definitions = _projectorRegistry.ListDefinitionsByEvent(eventMessage.Body);
                        var writeTasks = definitions.Select(definition => Task.Run(async () =>
                        {
                            var projectorId = definition.GenerateIdentifierFor(eventMessage.Body);
                            var proxy = _actorProxyFactory.CreateActorProxy<IProjectorActor>(_projectorActorUri, new ActorId(projectorId));
                            var projectorMessage = new ProjectorMessage(eventMessage.Body, definition.ProjectorType, eventMessage.Headers);

                            await proxy.Project(projectorMessage, cancellationToken);
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

        public async Task Publish(EventMessage[] messages, CancellationToken cancellationToken)
        {
            while (true)
            {
                try
                {
                    using (var transaction = this.StateManager.CreateTransaction())
                    {
                        foreach (var message in messages)
                        {
                            await Queue.EnqueueAsync(transaction, message, TimeSpan.FromMilliseconds(100), cancellationToken);
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
        }
    }
}