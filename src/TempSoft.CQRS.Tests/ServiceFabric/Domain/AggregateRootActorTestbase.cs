using FakeItEasy;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using ServiceFabric.Mocks;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.ServiceFabric.Domain;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Domain
{
    public abstract class AggregateRootActorTestBase
    {
        //protected const string EventStreamStateName = "_tempsoft_cqrs_event_stream";
        //protected const string CommandLogStateName = "_tempsoft_cqrs_command_log";
        //protected const string AggregateRootTypeStateName = "_tempsoft_cqrs_root_type";

        protected readonly IActorProxyFactory ActorProxyFactory = A.Fake<IActorProxyFactory>();
        protected readonly IServiceProxyFactory ServiceProxyFactory = A.Fake<IServiceProxyFactory>();
        protected readonly IAggregateRootRepository AggregateRootRepository = A.Fake<IAggregateRootRepository>();
        protected readonly MockActorService<AggregateRootActor> ActorService;

        protected AggregateRootActorTestBase()
        {
            ActorService = ServiceFabricFactories.CreateActorServiceForActorWithCustomStateManager<AggregateRootActor>(
                (service, id) => new AggregateRootActor(service, id, _ => AggregateRootRepository, ActorProxyFactory, ServiceProxyFactory));
        }
    }
}