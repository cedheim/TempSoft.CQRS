using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NCG.NGS.CQRS.Events;
using NCG.NGS.CQRS.ServiceFabric.Interfaces.Messaging;
using NCG.NGS.CQRS.Tests.Mocks;
using NUnit.Framework;

namespace NCG.NGS.CQRS.Tests.ServiceFabric.Events.EventBusService
{
    [TestFixture]
    public class When_publishing_events : EventBusServiceTestBase
    {
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {

            _events = new IEvent[]
            {
                new InitializationEvent(Data.RootId) {Version = 1},
                new ChangedAValue(5) {AggregateRootId = Data.RootId, Version = 2}
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

        [Test]
        public void Should_have_applied_the_event_to_the_query_builder_registry()
        {
            A.CallTo(() => Registry.Apply(A<IEvent>.That.Matches(e => e.Id == _events[0].Id)))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => Registry.Apply(A<IEvent>.That.Matches(e => e.Id == _events[1].Id)))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        private static class Data
        {
            public static readonly Guid RootId = Guid.NewGuid();
        }
    }
}