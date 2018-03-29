using FakeItEasy;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NCG.NGS.CQRS.Domain;
using NCG.NGS.CQRS.Queries;
using NCG.NGS.CQRS.ServiceFabric.Domain;
using NCG.NGS.CQRS.ServiceFabric.Queries;
using NCG.NGS.CQRS.Tests.Mocks;
using ServiceFabric.Mocks;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Queries
{
    public abstract class QueryBuilderActorTestBase
    {
        protected readonly IActorProxyFactory ActorProxyFactory = A.Fake<IActorProxyFactory>();
        protected readonly IServiceProxyFactory ServiceProxyFactory = A.Fake<IServiceProxyFactory>();
        protected readonly IQueryBuilderRegistry Registry = A.Fake<IQueryBuilderRegistry>();
        protected readonly MockActorService<QueryBuilderActor> ActorService;

        protected QueryBuilderActorTestBase()
        {
            ActorService = ServiceFabricFactories.CreateActorServiceForActorWithCustomStateManager<QueryBuilderActor>(
                (service, id) => new QueryBuilderActor(service, id, Registry, ActorProxyFactory, ServiceProxyFactory));
        }
    }
}