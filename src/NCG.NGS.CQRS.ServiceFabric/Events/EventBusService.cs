using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using NCG.NGS.CQRS.Common.Random;
using NCG.NGS.CQRS.Queries;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Events;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Events
{
    public class EventBusService : StatefulService, IEventBusService
    {
        private readonly IQueryBuilderRegistry _queryBuilderRegistry;
        private const string EventQueueName = "_ncg_ngs_event_queue";

        public EventBusService(StatefulServiceContext serviceContext, IQueryBuilderRegistry queryBuilderRegistry) : base(serviceContext)
        {
            _queryBuilderRegistry = queryBuilderRegistry;
        }

        public EventBusService(StatefulServiceContext serviceContext, IReliableStateManagerReplica reliableStateManagerReplica, IQueryBuilderRegistry queryBuilderRegistry) : base(serviceContext, reliableStateManagerReplica)
        {
            _queryBuilderRegistry = queryBuilderRegistry;
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            var queue = await StateManager.GetOrAddAsync<IReliableConcurrentQueue<EventMessage>>(EventQueueName);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using (var tx = this.StateManager.CreateTransaction())
                    {
                        var conditionalDequeue = await queue.TryDequeueAsync(tx, cancellationToken);
                        if (!conditionalDequeue.HasValue)
                        {
                            // if there is nothing to dequeue we wait for a while and retry.
                            await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                            continue;
                        }

                        var message = conditionalDequeue.Value;

                        await _queryBuilderRegistry.Apply(message.Body);

                        await tx.CommitAsync();
                    }
                }
                catch (TimeoutException e)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(ThreadSafeRandom.NextDouble() * 50.0 + 50.0), cancellationToken);
                }
                catch (FabricTransientException fte)
                {
                    // Transient exceptions can be retried without faulting the replica.
                    // Instead of retrying here, simply move on to the next iteration after a delay (set below).
                    await Task.Delay(TimeSpan.FromMilliseconds(ThreadSafeRandom.NextDouble() * 50.0 + 50.0), cancellationToken);
                }
                catch (FabricNotPrimaryException)
                {
                    // This replica is no longer primary, so we can exit gracefully here.
                    return;
                }
                catch (OperationCanceledException)
                {
                    // this means the service needs to shut down. Make sure it gets re-thrown.
                    throw;
                }
                catch (System.Exception e)
                {
                    throw;
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

            var queue = await StateManager.GetOrAddAsync<IReliableConcurrentQueue<EventMessage>>(EventQueueName);

            foreach (var eventMessage in events)
            {
                while (true)
                {
                    try
                    {
                        using (var transaction = this.StateManager.CreateTransaction())
                        {
                            await queue.EnqueueAsync(transaction, eventMessage, cancellationToken);
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

            stopwatch.Stop();
        }
    }
}