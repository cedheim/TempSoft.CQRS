using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;
using NUnit.Framework;
using TempSoft.CQRS.Events;
using TempSoft.CQRS.ServiceFabric.Events;
using TempSoft.CQRS.Tests.Mocks;

namespace TempSoft.CQRS.Tests.ServiceFabric.Events.ActorEventStore
{
    [TestFixture]
    public class When_getting_an_event_stream_from_the_event_store
    {
        private IActorStateManager _stateManager;
        private CQRS.ServiceFabric.Events.ActorEventStore _store;
        private IEvent[] _events;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            _stateManager = A.Fake<IActorStateManager>();
            _store = new CQRS.ServiceFabric.Events.ActorEventStore(_stateManager);

            A.CallTo(() => _stateManager.TryGetStateAsync<EventStream>(A<string>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .Returns(new ConditionalValue<EventStream>(true, new EventStream(new IEvent[] { new ChangedAValue(5) { Version = 2 }, new CreatedAThing(Data.ActorId) { Version = 1 } })));
            
            _events = (await _store.Get(Data.ActorId, 0, CancellationToken.None)).ToArray();
        }

        [Test]
        public void Should_be_able_to_get_a_stream()
        {
            _events.Should().NotBeNull();
            _events.Should().BeInAscendingOrder(e => e.Version);
        }

        [Test]
        public void Should_have_tried_to_get_the_state()
        {
            A.CallTo(() => _stateManager.TryGetStateAsync<EventStream>(A<string>.That.IsNotNull(), A<CancellationToken>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
        
        private static class Data
        {
            public static readonly Guid ActorId = Guid.NewGuid();
        }

    }
}