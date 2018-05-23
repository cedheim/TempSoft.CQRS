using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Events;
using TempSoft.CQRS.ServiceFabric.Interfaces.Messaging;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.EventBusService
{
    [TestFixture]
    public class When_publishing_events : EventBusServiceTestBase
    {
        private IEvent[] _events;
        private EventStreamDefinition _definition;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _definition = new EventStreamDefinition(Data.EventStreamName, new EventFilter());
            A.CallTo(() => Registry.GetEventStreamsByEvent(A<IEvent>.Ignored)).Returns(new[] {_definition});

            _events = new IEvent[]
            {
                new CreatedAThing(Data.RootId) {Version = 1}
            };

            var messages = _events.Select(e => new EventMessage(e)).ToArray();

            await Service.Publish(messages, CancellationToken.None);

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.CancelAfter(TimeSpan.FromMilliseconds(500));

            try
            {
                await Service.InvokeRunAsync(cancellationTokenSource.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }


        private static class Data
        {
            public const string EventStreamName = "EventStream";
            public static readonly Guid RootId = Guid.NewGuid();
        }

        [Test]
        public void Should_have_created_a_service_proxy()
        {
            A.CallTo(() => ServiceProxyFactory.CreateServiceProxy<IEventStreamService>(A<Uri>.Ignored,
                    A<ServicePartitionKey>.That.Matches(pk => pk.Value.ToString() == Data.EventStreamName),
                    A<TargetReplicaSelector>.Ignored, A<string>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_generated_the_uri_for_the_service()
        {
            A.CallTo(() => UriHelper.GetUriForSerivce<IEventStreamService>())
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public void Should_have_written_the_event_to_the_stream()
        {
            A.CallTo(() => StreamService.Write(A<EventMessage>.That.Matches(msg => msg.Body == _events[0]),
                    A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}