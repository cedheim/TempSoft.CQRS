using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NCG.NGS.CQRS.Domain;
using NCG.NGS.CQRS.ServiceFabric.Events;
using NCG.NGS.CQRS.ServiceFabric.Exceptions;
using NCG.NGS.CQRS.ServiceFabric.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Domain
{
    public class AggregateRootActor : Actor, IAggregateRootActor
    {
        private const string EventStreamStateName = "_ncg_ngs_cqrs_event_stream";

        public AggregateRootActor(ActorService actorService, ActorId actorId, IActorProxyFactory actorProxyFactory, IServiceProxyFactory serviceProxyFactory) : base(actorService, actorId)
        {
            ActorProxyFactory = actorProxyFactory;
            ServiceProxyFactory = serviceProxyFactory;
        }

        private IAggregateRoot _root;

        public IActorProxyFactory ActorProxyFactory { get; }

        public IServiceProxyFactory ServiceProxyFactory { get; }

        public async Task Initialize(InitializeMessage message, CancellationToken cancellationToken)
        {
            var id = this.GetActorId().GetGuidId();

            if (_root != null)
            {
                throw new DoubleInitializationException(id);
            }

            _root = Activate(message.AggregateRootType);
            _root.Initialize(id);

            await SaveAndDispatchEvents(cancellationToken);
        }

        public async Task Handle(CommandMessage message, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        private IAggregateRoot Activate(Type type)
        {
            return Activator.CreateInstance(type) as IAggregateRoot;
        }

        private async Task SaveAndDispatchEvents(CancellationToken cancellationToken)
        {
            var events = _root.Commit().ToArray();

            await StateManager.AddOrUpdateStateAsync(EventStreamStateName, new EventStream(events), (s, stream) =>
            {
                stream.AddRange(events);
                return stream;
            }, cancellationToken);
        }

    }
}