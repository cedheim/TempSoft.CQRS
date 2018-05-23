using System;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.ServiceFabricEventStream
{
    [TestFixture]
    public class When_reading_nothing_from_the_event_stream : ServiceFabricEventStreamTestBase
    {
        private EventStreamDefinition _definition;
        private ServiceFabricEventStreamFactory _factory;
        private IEventStream _reader;
        private IEvent _result;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _definition = new EventStreamDefinition(Data.EventStreamName, new EventFilter());

            A.CallTo(() => StreamRegistry.GetEventStreamByName(Data.EventStreamName))
                .Returns(_definition);

            A.CallTo(() => StreamService.Read(A<TimeSpan>.Ignored, A<CancellationToken>.Ignored))
                .Returns(default(EventMessage));

            _factory = new ServiceFabricEventStreamFactory(UriHelper, StreamRegistry, ServiceProxyFactory);
            _reader = await _factory.Open(Data.EventStreamName);

            _result = await _reader.Read(Data.ReadTimeout, CancellationToken.None);
        }

        [Test]
        public void Should_have_gotten_the_stream_from_the_registry()
        {
            A.CallTo(() => StreamRegistry.GetEventStreamByName(Data.EventStreamName))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_gotten_the_uri_for_the_service()
        {
            A.CallTo(() => UriHelper.GetUriForSerivce<IEventStreamService>())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_created_a_service_proxy_factory()
        {
            A.CallTo(() => ServiceProxyFactory.CreateServiceProxy<IEventStreamService>(StreamServiceUri, A<ServicePartitionKey>.That.Matches(pk => pk.Value.ToString() == Data.EventStreamName), A<TargetReplicaSelector>.Ignored, A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_read_from_the_service()
        {
            A.CallTo(() => StreamService.Read(Data.ReadTimeout, A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_returned_the_correct_result()
        {
            _result.Should().BeNull();
        }

        private static class Data
        {
            public const string EventStreamName = "Stream1";
            public static readonly TimeSpan ReadTimeout = TimeSpan.FromMilliseconds(100);
        }

    }
}