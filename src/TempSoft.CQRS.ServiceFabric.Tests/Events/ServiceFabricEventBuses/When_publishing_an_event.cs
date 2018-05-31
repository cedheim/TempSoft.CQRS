using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using NUnit.Framework;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.Mocks;
using TempSoft.CQRS.ServiceFabric.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.ServiceFabric.Tools;

namespace TempSoft.CQRS.ServiceFabric.Tests.Events.ServiceFabricEventBuses
{
    public class When_publishing_an_event
    {
        private IServiceProxyFactory _proxyFactory;
        private IUriHelper _uriHelper;
        private ServiceFabricEventBus _eventBus;
        private IEvent[] _events;
        private EventMessage[] _messages;
        private IEventBusService _eventBusService;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _uriHelper = A.Fake<IUriHelper>();
            _proxyFactory = A.Fake<IServiceProxyFactory>();
            _eventBusService = A.Fake<IEventBusService>();

            A.CallTo(() => _proxyFactory.CreateServiceProxy<IEventBusService>(A<Uri>.Ignored, A<ServicePartitionKey>.Ignored, A<TargetReplicaSelector>.Ignored, A<string>.Ignored))
                .Returns(_eventBusService);
            A.CallTo(() => _uriHelper.GetUriFor<IEventBusService>())
                .Returns(Data.Uri);
            A.CallTo(() => _eventBusService.Publish(A<EventMessage[]>.Ignored, A<CancellationToken>.Ignored))
                .Invokes(foc => _messages = foc.GetArgument<EventMessage[]>(0));


            _events = new IEvent[]
            {
                new ChangedAValue(5) { AggregateRootId = Data.AggregateRootId1, Version = 5 },
                new ChangedBValue("HELLO") { AggregateRootId = Data.AggregateRootId1, Version = 3 },
                new ChangedAValue(7) { AggregateRootId = Data.AggregateRootId1, Version = 4 },
            };


            _eventBus = new ServiceFabricEventBus(_proxyFactory, _uriHelper);
            await _eventBus.Publish(_events, CancellationToken.None);
        }

        [Test]
        public void Should_have_created_a_proxy_with_the_correct_uri()
        {
            var hash = Data.AggregateRootId1.GetHashCode64();

            A.CallTo(() => _proxyFactory.CreateServiceProxy<IEventBusService>(Data.Uri, A<ServicePartitionKey>.That.Matches(spk => ((long)spk.Value) == hash), A<TargetReplicaSelector>.Ignored, A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_published_the_events()
        {
            A.CallTo(() => _eventBusService.Publish(A<EventMessage[]>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_ordered_events()
        {
            _messages.Should().NotBeNull();
            _messages.Should().BeInAscendingOrder(msg => msg.Body.Version);
        }

        private static class Data
        {
            public static readonly Guid AggregateRootId1 = Guid.NewGuid();
            public static readonly Uri Uri = new Uri("fabric:/application/service");
        }


    }
}