using System;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Data.Collections;
using NUnit.Framework;
using TempSoft.CQRS.Common.Extensions;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Events;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.EventStreamService
{
    [TestFixture]
    public class When_creating_the_event_stream_service : EventStreamServiceTestBase
    {
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _events = new IEvent[] {new ChangedAValue(5){ EventGroup = nameof(AThingAggregateRoot) } };

            A.CallTo(() => EventStore.List(A<EventStoreFilter>.Ignored,A<Func<IEvent, CancellationToken, Task>>.Ignored, A<CancellationToken>.Ignored))
                .Invokes(foc =>
                {
                    var callback = foc.GetArgument<Func<IEvent, CancellationToken, Task>>(1);
                    var cancellationToken = foc.GetArgument<CancellationToken>(2);
                    foreach (var e in _events)
                    {
                        callback(e, cancellationToken).Wait(cancellationToken);
                    }
                });

            try
            {

                CancellationTokenSource tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(TimeSpan.FromMilliseconds(500));

                await Service.InvokeRunAsync(tokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        [Test]
        public void Should_have_queried_for_partition_information()
        {
            A.CallTo(() => QueryClient.GetPartitionAsync(PartitionId))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_gotten_the_event_stream_definition()
        {
            A.CallTo(() => EventStreamRegistry.GetEventStreamByName(EventStreamName))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_gotten_all_old_events()
        {
            A.CallTo(() => EventStore.List(A<EventStoreFilter>.That.Matches(filter => filter.EventGroups.Contains(EventStreamDefinition.Filter.EventGroups[0]) && filter.EventTypes.Contains(EventStreamDefinition.Filter.EventTypes[0].ToFriendlyName())), A<Func<IEvent, CancellationToken, Task>>.That.Not.IsNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_be_able_to_read_from_the_stream()
        {
            var eventMessage = await Service.Read(1000.0, CancellationToken.None);
            var @event = eventMessage.Body;

            @event.Should().BeEquivalentTo(_events[0]);
        }

        [Test]
        public void Should_have_set_the_state_to_initialized()
        {
            A.CallTo(() => StreamStateManager.AddToEventCountForStream(EventStreamName, 1))
                .MustHaveHappened(Repeated.Exactly.Times(_events.Length));

            A.CallTo(() => StreamStateManager.SetStatusForStream(EventStreamName, EventStreamStatus.Initializing))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => StreamStateManager.SetStatusForStream(EventStreamName, EventStreamStatus.Initialized))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private static class Data
        {

        }
    }
}