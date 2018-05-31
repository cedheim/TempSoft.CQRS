using System;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using ServiceFabric.Mocks;
using TempSoft.CQRS.Projectors;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Projectors;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.ServiceFabric.Tests.Events.EventBusServices
{
    public class EventBusServiceTestBase
    {
        protected readonly IActorProxyFactory ActorProxyFactory = A.Fake<IActorProxyFactory>();
        protected readonly IServiceProxyFactory ServiceProxyFactory = A.Fake<IServiceProxyFactory>();
        protected readonly IEventBusService Service;
        protected readonly IProjectorRegistry Registry = A.Fake<IProjectorRegistry>();
        protected readonly IUriHelper UriHelper = A.Fake<IUriHelper>();
        protected readonly IProjectorActor ProjectorActor = A.Fake<IProjectorActor>();

        protected EventBusServiceTestBase()
        {
            var context = MockStatefulServiceContextFactory.Default;
            var stateManager = new MockReliableStateManager();

            A.CallTo(() => UriHelper.GetUriFor<IProjectorActor>())
                .Returns(new Uri(Data.Uri));
            A.CallTo(() => ActorProxyFactory.CreateActorProxy<IProjectorActor>(A<Uri>.Ignored, A<ActorId>.Ignored, A<string>.Ignored))
                .Returns(ProjectorActor);

            Service = new CQRS.ServiceFabric.Events.EventBusService(context, stateManager, Registry, UriHelper, ActorProxyFactory, ServiceProxyFactory);
        }


        private static class Data
        {
            public const string Uri = "fabric:/application/actorservice";
        }
    }
}