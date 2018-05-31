using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.ServiceFabric.Commands
{
    public class ServiceFabricCommandRouter : ICommandRouter
    {
        private readonly IUriHelper _uriHelper;
        private readonly IActorProxyFactory _actorProxyFactory;

        public ServiceFabricCommandRouter(IUriHelper uriHelper, IActorProxyFactory actorProxyFactory)
        {
            _uriHelper = uriHelper;
            _actorProxyFactory = actorProxyFactory;
        }

        public async Task Handle<TAggregate>(Guid id, ICommand command,
            CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRoot
        {
            var uri = _uriHelper.GetUriFor<IAggregateRootActor>();
            var actor = _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(uri, new ActorId(id));

            await actor.Handle(new CommandMessage(typeof(TAggregate), command), cancellationToken);
        }

        public async Task<TReadModel> GetReadModel<TAggregate, TReadModel>(Guid id,
            CancellationToken cancellationToken = default(CancellationToken)) where TAggregate : IAggregateRootWithReadModel
            where TReadModel : IAggregateRootReadModel
        {
            var uri = _uriHelper.GetUriFor<IAggregateRootActor>();
            var actor = _actorProxyFactory.CreateActorProxy<IAggregateRootActor>(uri, new ActorId(id));

            var message = await actor.GetReadModel(new GetReadModelMessage(typeof(TAggregate)), cancellationToken);

            return message.GetReadModel<TReadModel>();
        }
    }
}