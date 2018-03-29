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
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Domain;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Domain
{
    public class AggregateRootActor : Actor, IAggregateRootActor
    {
        private readonly IAggregateRootRepository _aggregateRootRepository;
        private const string AggregateRootTypeStateName = "_ncg_ngs_cqrs_root_type";

        public AggregateRootActor(ActorService actorService, ActorId actorId, IAggregateRootRepository aggregateRootRepository, IActorProxyFactory actorProxyFactory, IServiceProxyFactory serviceProxyFactory) : base(actorService, actorId)
        {
            _aggregateRootRepository = aggregateRootRepository;
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

        public Task<ReadModelMessage> GetReadModel()
        {
            throw new NotImplementedException();
        }
    }
}