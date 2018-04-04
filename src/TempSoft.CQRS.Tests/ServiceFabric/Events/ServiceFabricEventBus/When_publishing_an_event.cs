using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NUnit.Framework;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.Common.Uri;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.ServiceFabricEventBus
{
    [TestFixture]
    public class When_publishing_an_event
    {
        private IServiceProxyFactory _serviceProxyFactory;
        private IEventBusService _eventBusService;
        private CQRS.ServiceFabric.Events.ServiceFabricEventBus _eventBus;
        private IUriHelper _uriHelper;
        private ChangedAValue _event;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _eventBusService = A.Fake<IEventBusService>();
            _serviceProxyFactory = A.Fake<IServiceProxyFactory>();
            _uriHelper = A.Fake<IUriHelper>();

            A.CallTo(() => _uriHelper.GetUriForSerivce<IEventBusService>()).Returns(Data.ServiceUri);
            A.CallTo(() => _serviceProxyFactory.CreateServiceProxy<IEventBusService>(A<Uri>.Ignored, A<ServicePartitionKey>.Ignored, A<TargetReplicaSelector>.Ignored, A<string>.Ignored)).Returns(_eventBusService);
            
            _eventBus = new CQRS.ServiceFabric.Events.ServiceFabricEventBus(_serviceProxyFactory, _uriHelper);

            _event = new ChangedAValue(5) {AggregateRootId = Data.RootId, Version = 5};

            await _eventBus.Publish(new [] { _event }, CancellationToken.None);
        }

        [Test]
        public void Should_have_called_the_uri_helper()
        {
            A.CallTo(() => _uriHelper.GetUriForSerivce<IEventBusService>())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_created_a_service_proxy()
        {
            A.CallTo(() => _serviceProxyFactory.CreateServiceProxy<IEventBusService>(A<Uri>.That.Matches(uri => uri == Data.ServiceUri), A<ServicePartitionKey>.That.Matches(spk => spk.Kind == ServicePartitionKind.Int64Range && ((long)spk.Value) == Data.Hash), A<TargetReplicaSelector>.Ignored, A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_published_the_events()
        {
            A.CallTo(() => _eventBusService.Publish(A<EventMessage[]>.That.Matches(es => es.Any(e => e.Body.Id == _event.Id)), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }


        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
            public static readonly long Hash = RootId.GetHashCode64();
            public static readonly Uri ServiceUri = new Uri("fabric:/Application/EventBusService");
        }

    }
}