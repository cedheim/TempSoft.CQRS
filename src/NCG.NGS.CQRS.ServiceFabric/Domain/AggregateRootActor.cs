using System;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NCG.NGS.CQRS.Domain;
using NCG.NGS.CQRS.ServiceFabric.Messaging;

namespace NCG.NGS.CQRS.ServiceFabric.Domain
{
    public class AggregateRootActor : Actor, IAggregateRootActor
    {
        public AggregateRootActor(ActorService actorService, ActorId actorId, IActorProxyFactory actorProxyFactory, IServiceProxyFactory serviceProxyFactory) : base(actorService, actorId)
        {
            ActorProxyFactory = actorProxyFactory;
            ServiceProxyFactory = serviceProxyFactory;
        }

        public IActorProxyFactory ActorProxyFactory { get; }

        public IServiceProxyFactory ServiceProxyFactory { get; }

        public Task Initialize(InitializeMessage message)
        {
            throw new System.NotImplementedException();
        }

        public Task Handle(CommandMessage message)
        {
            throw new System.NotImplementedException();
        }

        public IAggregateRoot Activate(Type type)
        {
            return Activator.CreateInstance(type) as IAggregateRoot;
        }
    }
}