using System.Collections.Generic;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NCG.NGS.CQRS.ServiceFabric.Domain;
using NCG.NGS.CQRS.Tests.Mocks;
using ServiceFabric.Mocks;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Domain
{
    public abstract class AggregateRootActorTestBase
    {
        protected const string EventStreamStateName = "_ncg_ngs_cqrs_event_stream";
        protected readonly IActorProxyFactory ActorProxyFactory = A.Fake<IActorProxyFactory>();
        protected readonly IServiceProxyFactory ServiceProxyFactory = A.Fake<IServiceProxyFactory>();
        protected readonly Dictionary<ActorId, InMemoryActorStateManager> StateManagers = new Dictionary<ActorId, InMemoryActorStateManager>();
        protected readonly MockActorService<AggregateRootActor> ActorService;
        protected readonly Dictionary<string, object> InitialState = new Dictionary<string, object>();

        protected AggregateRootActorTestBase()
        {
            ActorService = ServiceFabricFactories.CreateActorServiceForActorWithCustomStateManager<AggregateRootActor>(
                (service, id) => new AggregateRootActor(service, id, ActorProxyFactory, ServiceProxyFactory),
                stateManagerFactory: (actor, provider) => { StateManagers.Add(actor.Id, new InMemoryActorStateManager(InitialState)); return StateManagers[actor.Id]; });
        }
    }
}