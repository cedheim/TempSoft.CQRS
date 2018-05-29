using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TempSoft.CQRS.Projectors;

namespace TempSoft.CQRS.Events
{
    public class InMemoryEventBus : IEventBus, IDisposable
    {
        private readonly IProjectorRegistry _projectorRegistry;
        private readonly IProjectorRepository _projectorRepository;
        private readonly ConcurrentQueue<IEvent> _queue;
        private readonly Thread _worker;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public InMemoryEventBus(IProjectorRegistry projectorRegistry, IProjectorRepository projectorRepository)
        {
            _projectorRegistry = projectorRegistry;
            _projectorRepository = projectorRepository;
            _cancellationTokenSource = new CancellationTokenSource();
            _queue = new ConcurrentQueue<IEvent>();
            _worker =  new Thread(Run);
            _worker.Start(_cancellationTokenSource.Token);
        }

        public async Task Publish(IEnumerable<IEvent> events, CancellationToken cancellationToken)
        {
            foreach(var @event in events)
                _queue.Enqueue(@event);
        }

        private async Task QueueManager(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (_queue.TryDequeue(out var @event))
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                        continue;
                    }

                    var definitions = _projectorRegistry.ListDefinitionsByEvent(@event);

                    await Task.WhenAll(definitions.Select(definition => Task.Run(async () =>
                    {
                        var id = definition.GenerateIdentifierFor(@event);
                        var projector = await _projectorRepository.Get(definition.ProjectorType, id, cancellationToken);

                        await projector.Project(@event, cancellationToken);
                    })));
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception) { }
            }
        }

        private void Run(object arg)
        {
            var cancellationToken = (CancellationToken) arg;
            QueueManager(cancellationToken).Wait();
        }
        
        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            while(_worker.IsAlive)
                Thread.Sleep(100);
        }
    }
}