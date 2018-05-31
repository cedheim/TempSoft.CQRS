using FakeItEasy;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using ServiceFabric.Mocks;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.ServiceFabric.Domain;
using TempSoft.CQRS.ServiceFabric.Projectors;
using TempSoft.CQRS.ServiceFabric.Tests.Mocks;

namespace TempSoft.CQRS.ServiceFabric.Tests.Projectors
{
    public abstract class ProjectorActorTestBase
    {
        protected IActorProxyFactory ActorProxyFactory;
        protected IServiceProxyFactory ServiceProxyFactory;
        protected IProjectorRepository ProjectorRepository;
        protected IProjectionModelRepository ProjectionModelRepository;
        protected MockActorService<ProjectorActor> ActorService;

        protected ProjectorActorTestBase()
        {
            ActorProxyFactory = A.Fake<IActorProxyFactory>();
            ServiceProxyFactory = A.Fake<IServiceProxyFactory>();
            ProjectorRepository = A.Fake<IProjectorRepository>();
            ProjectionModelRepository = A.Fake<IProjectionModelRepository>();
            ActorService = ServiceFabricFactories.CreateActorServiceForActorWithCustomStateManager<ProjectorActor>((service, id) => new ProjectorActor(service, id, ProjectorRepository, ActorProxyFactory, ServiceProxyFactory));
        }

    }
}