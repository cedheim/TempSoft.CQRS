using FakeItEasy;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NCG.NGS.CQRS.Queries;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Events;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Queries;
using ServiceFabric.Mocks;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Events.EventBusService
{
    public abstract class EventBusServiceTestBase
    {
        protected readonly IActorProxyFactory ActorProxyFactory = A.Fake<IActorProxyFactory>();
        protected readonly IServiceProxyFactory ServiceProxyFactory = A.Fake<IServiceProxyFactory>();
        protected readonly IEventBusService Service;
        protected readonly IQueryBuilderRegistry Registry = new QueryBuilderRegistry();
        protected readonly IQueryBuilderActor Actor = A.Fake<IQueryBuilderActor>();

        protected EventBusServiceTestBase()
        {
            var context = MockStatefulServiceContextFactory.Default;
            var stateManager = new MockReliableStateManager();

            Service = new CQRS.ServiceFabric.Events.EventBusService(context, stateManager, Registry, ServiceProxyFactory, ActorProxyFactory);
        }


    }
}
