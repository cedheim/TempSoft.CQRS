using System.Collections.Generic;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NCG.NGS.CQRS.Domain;
using NCG.NGS.CQRS.ServiceFabric.Domain;
using NCG.NGS.CQRS.Tests.Mocks;
using ServiceFabric.Mocks;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Domain
{
    public abstract class AggregateRootActorTestBase
    {
        //protected const string EventStreamStateName = "_ncg_ngs_cqrs_event_stream";
        //protected const string CommandLogStateName = "_ncg_ngs_cqrs_command_log";
        //protected const string AggregateRootTypeStateName = "_ncg_ngs_cqrs_root_type";

        protected readonly IActorProxyFactory ActorProxyFactory = A.Fake<IActorProxyFactory>();
        protected readonly IServiceProxyFactory ServiceProxyFactory = A.Fake<IServiceProxyFactory>();
        protected readonly IRepository Repository = A.Fake<IRepository>();
        protected readonly MockActorService<AggregateRootActor> ActorService;

        protected AggregateRootActorTestBase()
        {
            ActorService = ServiceFabricFactories.CreateActorServiceForActorWithCustomStateManager<AggregateRootActor>(
                (service, id) => new AggregateRootActor(service, id, Repository, ActorProxyFactory, ServiceProxyFactory));
        }
    }
}