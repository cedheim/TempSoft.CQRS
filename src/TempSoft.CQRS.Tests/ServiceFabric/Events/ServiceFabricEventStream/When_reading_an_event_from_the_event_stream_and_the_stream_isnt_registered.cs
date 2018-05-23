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
using TempSoft.CQRS.ServiceFabric.Exceptions;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.ServiceFabricEventStream
{
    [TestFixture]
    public class When_reading_an_event_from_the_event_stream_and_the_stream_isnt_registered : ServiceFabricEventStreamTestBase
    {
        private ServiceFabricEventStreamFactory _factory;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            A.CallTo(() => StreamRegistry.GetEventStreamByName(Data.EventStreamName))
                .Returns(default(EventStreamDefinition));

            _factory = new ServiceFabricEventStreamFactory(UriHelper, StreamRegistry, ServiceProxyFactory);
            
            _factory.Invoking(f => f.Open(Data.EventStreamName).GetAwaiter().GetResult()).Should().Throw<EventStreamNotFoundException>();
        }

        [Test]
        public void Should_have_gotten_the_stream_from_the_registry()
        {
            A.CallTo(() => StreamRegistry.GetEventStreamByName(Data.EventStreamName))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private static class Data
        {
            public const string EventStreamName = "Stream1";
            public static readonly TimeSpan ReadTimeout = TimeSpan.FromMilliseconds(100);
        }

    }
}