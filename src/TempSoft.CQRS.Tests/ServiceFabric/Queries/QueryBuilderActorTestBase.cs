using FakeItEasy;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using ServiceFabric.Mocks;
using TempSoft.CQRS.Queries;
using TempSoft.CQRS.ServiceFabric.Queries;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Queries
{
    public abstract class QueryBuilderActorTestBase
    {
        protected readonly IActorProxyFactory ActorProxyFactory = A.Fake<IActorProxyFactory>();
        protected readonly MockActorService<QueryBuilderActor> ActorService;
        protected readonly IQueryBuilderRegistry Registry = A.Fake<IQueryBuilderRegistry>();
        protected readonly IServiceProxyFactory ServiceProxyFactory = A.Fake<IServiceProxyFactory>();

        protected QueryBuilderActorTestBase()
        {
            ActorService = ServiceFabricFactories.CreateActorServiceForActorWithCustomStateManager<QueryBuilderActor>(
                (service, id) => new QueryBuilderActor(service, id, Registry, ActorProxyFactory, ServiceProxyFactory));
        }
    }
}