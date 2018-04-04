using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Domain;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;

namespace TempSoft.CQRS.ServiceFabric.Domain
{
    public class AggregateRootActor : Actor, IAggregateRootActor
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private const string AggregateRootTypeStateName = "_tempsoft_cqrs_root_type";

        public AggregateRootActor(ActorService actorService, ActorId actorId, Func<AggregateRootActor, IAggregateRootRepository> aggregateRootRepositoryFactory, IActorProxyFactory actorProxyFactory, IServiceProxyFactory serviceProxyFactory) : base(actorService, actorId)
        {
            _aggregateRootRepository = aggregateRootRepositoryFactory(this);

            ActorProxyFactory = actorProxyFactory;
            ServiceProxyFactory = serviceProxyFactory;
        }

        protected override async Task OnActivateAsync()
        {
            await base.OnActivateAsync();
        }

        private IAggregateRoot _root;

        public IActorProxyFactory ActorProxyFactory { get; }

        public IServiceProxyFactory ServiceProxyFactory { get; }
        
        public async Task Initialize(InitializeMessage message, CancellationToken cancellationToken)
        {
            var id = this.GetActorId().GetGuidId();

            _root = await _aggregateRootRepository.Get(message.AggregateRootType, id, cancellationToken);
            _root.Initialize(id);

            await _aggregateRootRepository.Save(_root, cancellationToken);
        }

        public async Task Handle(CommandMessage message, CancellationToken cancellationToken)
        {
            var id = this.GetActorId().GetGuidId();

            // should we initialize if we receive a command before it has been initialized?
            if (_root == null)
            {
                _root = await _aggregateRootRepository.Get(message.AggregateRootType, id, cancellationToken);
            }

            var command = message.Body;
            
            _root.Handle(command);

            await _aggregateRootRepository.Save(_root, cancellationToken);
        }

        public async Task<ReadModelMessage> GetReadModel(GetReadModelMessage message, CancellationToken cancellationToken)
        {
            var id = this.GetActorId().GetGuidId();

            // should we initialize if we receive a command before it has been initialized?
            if (_root == null)
            {
                _root = await _aggregateRootRepository.Get(message.AggregateRootType, id, cancellationToken);
            }

            if (_root is IAggregateRootWithReadModel rootWithReadModel)
            {
                var readModel = rootWithReadModel.GetReadModel();
                return new ReadModelMessage(readModel);
            }

            throw new NotImplementedException();
        }
    }
}