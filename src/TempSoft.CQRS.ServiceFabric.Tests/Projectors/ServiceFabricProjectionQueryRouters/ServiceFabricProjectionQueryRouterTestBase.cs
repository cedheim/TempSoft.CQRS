using System;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using TempSoft.CQRS.ServiceFabric.Interfaces.Projectors;
using TempSoft.CQRS.ServiceFabric.Projectors;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.ServiceFabric.Tests.Projectors.ServiceFabricProjectionQueryRouters
{
    public abstract class ServiceFabricProjectionQueryRouterTestBase
    {
        protected readonly IActorProxyFactory ActorProxyFactory = A.Fake<IActorProxyFactory>();
        protected readonly IUriHelper UriHelper = A.Fake<IUriHelper>();
        protected readonly IProjectorActor Actor = A.Fake<IProjectorActor>();

        protected ServiceFabricProjectionQueryRouterTestBase()
        {
            A.CallTo(() => UriHelper.GetUriFor<IProjectorActor>())
                .Returns(new Uri("fabric:/application/service"));
            A.CallTo(() => ActorProxyFactory.CreateActorProxy<IProjectorActor>(A<Uri>.Ignored, A<ActorId>.Ignored, A<string>.Ignored))
                .Returns(Actor);
        }
    }
}