using System;
using FakeItEasy;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using ServiceFabric.Mocks;
using TempSoft.CQRS.Common.Uri;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Queries;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Queries;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.EventBusService
{
    public abstract class EventBusServiceTestBase
    {
        protected readonly IActorProxyFactory ActorProxyFactory = A.Fake<IActorProxyFactory>();
        protected readonly IServiceProxyFactory ServiceProxyFactory = A.Fake<IServiceProxyFactory>();
        protected readonly IEventBusService Service;
        protected readonly IEventStreamRegistry Registry = A.Fake<IEventStreamRegistry>();
        protected readonly IEventStreamService StreamService = A.Fake<IEventStreamService>();
        protected readonly IUriHelper UriHelper = A.Fake<IUriHelper>();

        protected EventBusServiceTestBase()
        {
            var context = MockStatefulServiceContextFactory.Default;
            var stateManager = new MockReliableStateManager();

            A.CallTo(() => ServiceProxyFactory.CreateServiceProxy<IEventStreamService>(A<Uri>.Ignored, A<ServicePartitionKey>.Ignored, A<TargetReplicaSelector>.Ignored, A<string>.Ignored))
                .Returns(StreamService);
            A.CallTo(() => UriHelper.GetUriForSerivce<IEventStreamService>())
                .Returns(new Uri(Data.Uri));

            Service = new CQRS.ServiceFabric.Events.EventBusService(context, stateManager, Registry, UriHelper, ServiceProxyFactory, ActorProxyFactory);
        }

        private static class Data
        {
            public const string Uri = "fabric:/application/service";
        }
    }
}
