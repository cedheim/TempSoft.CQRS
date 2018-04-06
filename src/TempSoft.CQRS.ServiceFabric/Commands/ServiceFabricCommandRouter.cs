using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Commands
{
    public class ServiceFabricCommandRouter : ICommandRouter
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public ServiceFabricCommandRouter(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory;
        }
        
        public async Task Handle<TAggregate>(Guid id, ICommand command, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot
        {
            var actor = _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(new ActorId(id));

            await actor.Handle(new CommandMessage(typeof(TAggregate), command), cancellationToken);
        }

        public async Task<TReadModel> GetReadModel<TAggregate, TReadModel>(Guid id, CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot where TReadModel : IAggregateRootReadModel
        {
            var actor = _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(new ActorId(id));

            var message = await actor.GetReadModel(new GetReadModelMessage(typeof(TAggregate)), cancellationToken);

            return message.GetReadModel<TReadModel>();
        }
    }
}