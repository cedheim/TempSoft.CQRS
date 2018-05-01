using System;
using System.Diagnostics;
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
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.EventStreamService
{
    [TestFixture]
    public class When_reading_from_the_event_stream_and_there_is_nothing_there : EventStreamServiceTestBase
    {
        private IEvent[] _events;
        private EventStreamDefinition _definition;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _events = new IEvent[0];

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
        }

        [Test]
        public async Task Should_return_after_timeout()
        {
            var timeout = TimeSpan.FromMilliseconds(500);

            var stopwatch = new Stopwatch();

            stopwatch.Start();

            var eventMessage = await Service.Read(timeout.TotalMilliseconds, CancellationToken.None);

            stopwatch.Stop();

            eventMessage.Should().BeNull();

            stopwatch.Elapsed.Should().BeCloseTo(timeout, 100);
        }

        private static class Data
        {

        }
    }
}