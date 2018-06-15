using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.ServiceFabric.Exceptions;
using TempSoft.CQRS.ServiceFabric.Interfaces.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Domain
{
    public class AggregateRootActor : Actor, IAggregateRootActor
    {
        private const string AggregateRootTypeStateName = "_tempsoft_cqrs_root_type";
        private readonly IAggregateRootRepository _aggregateRootRepository;

        private IAggregateRoot _root;

        public AggregateRootActor(ActorService actorService, ActorId actorId,
            IAggregateRootRepository aggregateRootRepository,
            IActorProxyFactory actorProxyFactory, IServiceProxyFactory serviceProxyFactory) : base(actorService,
            actorId)
        {
            _aggregateRootRepository = aggregateRootRepository;

            ActorProxyFactory = actorProxyFactory;
            ServiceProxyFactory = serviceProxyFactory;
        }

        public IActorProxyFactory ActorProxyFactory { get; }

        public IServiceProxyFactory ServiceProxyFactory { get; }

        public async Task Handle(CommandMessage message, CancellationToken cancellationToken)
        {
            try
            {
                var id = this.GetActorId().GetGuidId();

                // should we initialize if we receive a command before it has been initialized?
                if (_root == null)
                    _root = await _aggregateRootRepository.Get(message.AggregateRootType, id, cancellationToken);

                var command = message.Body;

                await _root.Handle(command, cancellationToken);

                if (_root.Id != id)
                    throw new AggregateRootHasWrongIdException($"Has id {_root.Id} but should have id {id}");

                await _aggregateRootRepository.Save(_root, cancellationToken);
            }
            catch (Exception e)
            {
                // in case of an exception we reset the aggregate and the next time it will be loaded to the last known state.
                _root = default(IAggregateRoot);
                throw;
            }
        }

        public async Task<ReadModelMessage> GetReadModel(GetReadModelMessage message,
            CancellationToken cancellationToken)
        {
            var id = this.GetActorId().GetGuidId();

            if (_root == null)
                _root = await _aggregateRootRepository.Get(message.AggregateRootType, id, false, cancellationToken);

            if (_root == null)
                return new ReadModelMessage(null);

            if (_root is IAggregateRootWithReadModel rootWithReadModel)
            {
                var readModel = rootWithReadModel.GetReadModel();
                return new ReadModelMessage(readModel);
            }

            throw new NotImplementedException();
        }

        protected override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
        }
    }
}