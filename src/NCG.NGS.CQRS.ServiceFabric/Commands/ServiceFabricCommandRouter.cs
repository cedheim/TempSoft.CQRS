using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using NCG.NGS.CQRS.Commands;
using NCG.NGS.CQRS.Domain;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Domain;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Commands
{
    public class ServiceFabricCommandRouter : ICommandRouter
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public ServiceFabricCommandRouter(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory;
        }

        public async Task<Guid> Initialize<TAggregate>(CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot
        {
            var id = Guid.NewGuid();
            var actor = _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(new ActorId(id));

            await actor.Initialize(new InitializeMessage(typeof(TAggregate)), cancellationToken);

            return id;
        }

        public async Task Handle<TAggregate>(Guid id, ICommand command, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot
        {
            var actor = _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(new ActorId(id));

            await actor.Handle(new CommandMessage(typeof(TAggregate), command), cancellationToken);
        }
    }
}