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
using NCG.NGS.CQRS.ServiceFabric.Extensions;
using NCG.NGS.CQRS.ServiceFabric.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Domain
{
    public class AggregateRootActor : Actor, IAggregateRootActor
    {
        private readonly IRepository _repository;
        private const string EventStreamStateName = "_ncg_ngs_cqrs_event_stream";
        private const string CommandLogStateName = "_ncg_ngs_cqrs_command_log";
        private const string AggregateRootTypeStateName = "_ncg_ngs_cqrs_root_type";

        public AggregateRootActor(ActorService actorService, ActorId actorId, IRepository repository, IActorProxyFactory actorProxyFactory, IServiceProxyFactory serviceProxyFactory) : base(actorService, actorId)
        {
            _repository = repository;
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

            _root = await _repository.Get(message.AggregateRootType, id, cancellationToken);
            _root.Initialize(id);

            await _repository.Save(_root, cancellationToken);
        }

        public async Task Handle(CommandMessage message, CancellationToken cancellationToken)
        {
            var id = this.GetActorId().GetGuidId();

            // should we initialize if we receive a command before it has been initialized?
            if (_root == null)
            {
                _root = await _repository.Get(message.AggregateRootType, id, cancellationToken);
            }

            var command = message.Body;
            
            _root.Handle(command);

            await _repository.Save(_root, cancellationToken);
        }
    }
}