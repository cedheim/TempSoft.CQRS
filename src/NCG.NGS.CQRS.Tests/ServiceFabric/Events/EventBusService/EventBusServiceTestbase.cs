using FakeItEasy;
using NCG.NGS.CQRS.Queries;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Events;
using ServiceFabric.Mocks;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Events.EventBusService
{
    public abstract class EventBusServiceTestBase
    {
        protected readonly IEventBusService Service;
        protected readonly IQueryBuilderRegistry Registry = A.Fake<IQueryBuilderRegistry>();

        protected EventBusServiceTestBase()
        {
            var context = MockStatefulServiceContextFactory.Default;
            var stateManager = new MockReliableStateManager();
            Service = new CQRS.ServiceFabric.Events.EventBusService(context, stateManager, Registry);
        }


    }
}
