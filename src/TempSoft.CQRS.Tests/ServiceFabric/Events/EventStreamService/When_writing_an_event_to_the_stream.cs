using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Data.Collections;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.EventStreamService
{
    [TestFixture]
    public class When_writing_an_event_to_the_stream : EventStreamServiceTestBase
    {
        private IEvent _event;
        private EventMessage _result;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _event = new ChangedAValue(5) {EventGroup = nameof(AThingAggregateRoot)};

            await Service.Write(new CQRS.ServiceFabric.Interfaces.Messaging.EventMessage(_event), CancellationToken.None);

            _result = await Service.Read(TimeSpan.FromMilliseconds(500), CancellationToken.None);
        }

        [Test]
        public void Should_have_queried_for_partition_information()
        {
            var @event = _result.Body;

            @event.Should().BeEquivalentTo(_event);
        }


        [Test]
        public void Should_have_set_the_state_to_initialized()
        {
            A.CallTo(() => StreamStateManager.AddToEventCountForStream(EventStreamName, 1))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private static class Data
        {

        }
    }
}